using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public Camera camera;
    public List<GameObject> characterAssets = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane hPlane = new Plane(Vector3.up, Vector3.zero);
            float distance = 0;
            if (hPlane.Raycast(ray, out distance))
            {
                GameObject go2 = tileBehaviour.getClosestTile(ray.GetPoint(distance));
                if (go2 != null)
                {
                    GameObject go = Instantiate(characterAssets[(int)Random.Range(0f, characterAssets.Count - 0.1f)], new Vector3(0, 0, 0), Quaternion.identity);
                    go.transform.position = go2.transform.position;
                }
            }
        }
    }
}
