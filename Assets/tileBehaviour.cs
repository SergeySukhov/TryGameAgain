using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileBehaviour : MonoBehaviour
{
    public static List<GameObject> tiles = new List<GameObject>();

    public List<tileBehaviour> Neighbours = new List<tileBehaviour>(6);

    public static GameObject getClosestTile(Vector3 point)
    {
        GameObject go = null;
        float min = float.PositiveInfinity;
        foreach(var tile in tiles)
        {
            var tmp = (tile.transform.position - point).sqrMagnitude;
            if (tmp < min)
            {
                go = tile;
                min = tmp;
            }
        }
        return go;
    }
    // Start is called before the first frame update
    void Start()
    {
        tiles.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
