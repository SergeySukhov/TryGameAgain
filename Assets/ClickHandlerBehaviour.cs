using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandlerBehaviour : MonoBehaviour, IGameStateReacting, IPawnAssetSelectorListener
{
    public Dictionary<GameStates, Action> gameStateUpdateMapping { get; set; } = new Dictionary<GameStates, Action>();
    public GameObject PawnAsset { get; set; }

    FieldBehaviour Field = null;

    GameObject gameObjectClicked;

    public void OnStateSwitched(GameStates currentState, GameStates previousState)
    {
        CurrentUpdate = gameStateUpdateMapping[currentState];
        PawnAsset = null;
    }

    Action CurrentUpdate = null;

    // Start is called before the first frame update
    void Start()
    {
        gameStateUpdateMapping.Add(GameStates.Acting, ActingUpdate);
        gameStateUpdateMapping.Add(GameStates.Pause, PauseUpdate);
        gameStateUpdateMapping.Add(GameStates.Planning, PlanningUpdate);
        Field = GameObject.FindGameObjectWithTag("Field").GetComponent<FieldBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentUpdate?.Invoke();
    }

    void PauseUpdate()
    {

    }

    void PlanningUpdate()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hitInfo.transform.gameObject.tag == "Pawn")
        {
            gameObjectClicked = hitInfo.transform.gameObject;
        }
        if (PawnAsset != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hitInfo.transform.gameObject.tag != "Pawn")
                {
                    PlacePawn();
                }
            }
        }
        else if (gameObjectClicked != null)
        {
            if (gameObjectClicked.tag == "Pawn")
                if (Input.GetMouseButtonDown(0))
                {
                    ControllPawn();
                }
        }
    }

    private void ControllPawn()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hitInfo.transform.gameObject.tag == "Ground")
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane hPlane = new Plane(Vector3.up, Vector3.zero);
            float distance = 0;
            if (hPlane.Raycast(ray, out distance))
            {
                GameObject go2 = Field.getClosestTile(ray.GetPoint(distance));
                if (go2 != null)
                {
                    PawnBehaviour pb = gameObjectClicked.GetComponent<PawnBehaviour>();
                    pb.MoveOnTile(go2);
                }
            }            
        } else if (hitInfo.transform.gameObject.tag == "Pawn")
        {
            gameObjectClicked = hitInfo.transform.gameObject;
        }
    }

    private void PlacePawn()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        float distance = 0;
        if (hitInfo.transform.gameObject.tag == "Ground")
        {
            if (hPlane.Raycast(ray, out distance))
            {
                GameObject go2 = Field.getClosestTile(ray.GetPoint(distance));
                if (go2 != null)
                {
                    GameObject go = Instantiate(PawnAsset, new Vector3(0, 0, 0), Quaternion.identity);
                    go.transform.position = go2.transform.position;
                    PawnAsset = null;
                }
            }
        }
        else if(hitInfo.transform.gameObject.tag == "Pawn")
        {
            gameObjectClicked = hitInfo.transform.gameObject;
        }
        PawnAsset = null;
    }

    void ActingUpdate()
    {

    }

    public void OnPawnAssetSelect(GameObject pawn)
    {
        PawnAsset = pawn;
    }
}
