using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public List<TileBehaviour> tiles { get; } = new List<TileBehaviour>();
    void Start()
    {
        CreateNavMesh();
    }

    public void OnTileCreated(TileBehaviour tile)
    {
        tiles.Add(tile);
    }

    public GameObject getClosestTile(Vector3 point)
    {
        GameObject go = null;
        float min = float.PositiveInfinity;
        foreach (var tile in tiles)
        {
            var tmp = (tile.transform.position - point).sqrMagnitude;
            if (tmp < min)
            {
                go = tile.gameObject;
                min = tmp;
            }
        }
        return go;
    }

    private void CreateNavMesh()
    {
        for (int i = 0; i < tiles.Count; ++i)
        {
            for (int j = i + 1; j < tiles.Count; ++j)
            {
                if ((tiles[i].transform.position - tiles[j].transform.position).sqrMagnitude < 3.0625f)
                {
                    var bi = tiles[i].GetComponent<TileBehaviour>();
                    var bj = tiles[j].GetComponent<TileBehaviour>();
                    bi.neighbours.Add(bj);
                    bj.neighbours.Add(bi);
                }
            }
        }
    }
}
