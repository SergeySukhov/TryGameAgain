using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum State
{
    Default = 1,
    PlacingCharacter,
    SelectedPawn,
}

[Serializable]
public class CharacterSelectEvent : UnityEvent<GameObject> { }

public class characterCreator : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject selectedAsset = null;
    GameObject movingPawn = null;
    FieldBehaviour Field = null;
    State CurrentState = State.Default;

    void Start()
    {
        Field = GameObject.FindGameObjectWithTag("Field").GetComponent<FieldBehaviour>();
    }

    void DefaultUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (hit && hitInfo.transform.gameObject.tag == "Pawn")
            {
                //назначение куда бежать
                CurrentState = State.SelectedPawn;
                movingPawn = hitInfo.transform.gameObject;
            }
            else
            {
                if (selectedAsset != null)
                {
                    Plane hPlane = new Plane(Vector3.up, Vector3.zero);
                    float distance = 0;
                    if (hPlane.Raycast(ray, out distance))
                    {
                        GameObject go2 = Field.getClosestTile(ray.GetPoint(distance));
                        if (go2 != null)
                        {
                            GameObject go = Instantiate(selectedAsset, new Vector3(0, 0, 0), Quaternion.identity);
                            go.transform.position = go2.transform.position;
                        }
                    }
                    selectedAsset = null;
                }
            }
        }
    }

    void PlacingCharacterUpdate()
    {
        if (selectedAsset != null && Input.GetMouseButton(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane hPlane = new Plane(Vector3.up, Vector3.zero);
            float distance = 0;
            if (hPlane.Raycast(ray, out distance))
            {
                GameObject go2 = Field.getClosestTile(ray.GetPoint(distance));
                if (go2 != null)
                {
                    GameObject go = Instantiate(selectedAsset, new Vector3(0, 0, 0), Quaternion.identity);
                    go.transform.position = go2.transform.position;
                }
            }
            selectedAsset = null;
            CurrentState = State.Default;
        }
    }

    void SelectedPawnUpdate()
    {
        if (movingPawn != null && Input.GetMouseButton(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane hPlane = new Plane(Vector3.up, Vector3.zero);
            float distance = 0;
            if (hPlane.Raycast(ray, out distance))
            {
                GameObject go2 = Field.getClosestTile(ray.GetPoint(distance));
                if (go2 != null)
                {
                    movingPawn.transform.position = go2.transform.position;
                }
            }
            movingPawn = null;
            CurrentState = State.Default;
        }
    }

    void HandleRunCommand()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case State.Default:
                HandleAssetSelection();
                DefaultUpdate();
                break;
            case State.PlacingCharacter:
                PlacingCharacterUpdate();
                break;
            case State.SelectedPawn:
                SelectedPawnUpdate();
                break;
            default:
                break;
        }
        
    }

    public List<GameObject> CharacterAssets = new List<GameObject>();
    // Start is called before the first frame update
    public CharacterSelectEvent CharacterSelectEvent = new CharacterSelectEvent();
    private void HandleAssetSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (CharacterAssets.Count > 0)
            {
                CharacterSelectEvent.Invoke(CharacterAssets[0]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (CharacterAssets.Count > 1)
            {
                CharacterSelectEvent.Invoke(CharacterAssets[1]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (CharacterAssets.Count > 2)
            {
                CharacterSelectEvent.Invoke(CharacterAssets[2]);
            }
        }
    }

    public void OnAssetSwitched(GameObject go)
    {
        selectedAsset = go;
        CurrentState = State.PlacingCharacter;
    }
}
