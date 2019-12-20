using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class TileEvent : UnityEvent<TileBehaviour> { }
public class TileBehaviour : MonoBehaviour
{
    public TileEvent tileCreated = new TileEvent();

    public bool isVisited = false;

    public int rate = int.MaxValue;

    public List<TileBehaviour> neighbours = new List<TileBehaviour>();
    // Start is called before the first frame update
    void Awake()
    {
        tileCreated.Invoke(this);
    }
}
