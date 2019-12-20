using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public interface IPawnListener
{
    void OnPawnCreated(GameObject pawn);
    void OnPawnDestroyed(GameObject pawn);
    void OnPawnStartActing(GameObject pawn);
    void OnPawnFinishedActing(GameObject pawn);
}
public class PawnBehaviour : MonoBehaviour, IGameStateReacting
{

    Action UpdateFunction = null;

    public Dictionary<GameStates, Action> gameStateUpdateMapping { get; set; }

    public List<TileBehaviour> tilesToVisit = new List<TileBehaviour>();

    public List<Vector3> path = null;

    FieldBehaviour Field = null;

    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        gameStateUpdateMapping = new Dictionary<GameStates, Action>();
        gameStateUpdateMapping.Add(GameStates.Acting, ActUpdate);
        gameStateUpdateMapping.Add(GameStates.Pause, PauseUpdate);
        gameStateUpdateMapping.Add(GameStates.Planning, PlanningUpdate);
        Field = GameObject.FindGameObjectWithTag("Field").GetComponent<FieldBehaviour>();
        GameStateSwitcher gss = Camera.main.GetComponent<GameStateSwitcher>();
        gss.StateSwitched.AddListener(OnStateSwitched);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFunction?.Invoke();
    }

    int curIdx = 0;
    void ActUpdate()
    {
        if (path == null)
        {
            path = GetPath(tilesToVisit);
            curIdx = 0;
        }
        
        if (path.Count > 0)
        {
            var pathLength = speed * Time.deltaTime;
            var curPos = transform.position;
            var segment = (path[curIdx] - curPos).magnitude;
            if ((curPos - path[path.Count - 1]).sqrMagnitude > 0.01f)
            {
                while (segment < pathLength && curIdx + 1 < path.Count)
                {
                    segment += (path[curIdx + 1] - path[curIdx]).magnitude;
                    curIdx++;
                }
                if (pathLength > segment && curIdx + 1 >= path.Count)
                {
                    gameObject.transform.position = path[path.Count - 1];
                }
                else if (curIdx > 0)
                {
                    gameObject.transform.position = path[curIdx] + (path[curIdx - 1] - path[curIdx]).normalized * (segment - pathLength);
                    gameObject.transform.forward = (path[curIdx] - path[curIdx - 1]).normalized;
                }
                else if (curIdx == 0)
                {
                    gameObject.transform.position = path[curIdx] + (curPos - path[0]).normalized * (segment - pathLength);
                    gameObject.transform.forward = (path[0] - curPos).normalized;
                }
                gameObject.GetComponent<Animator>().SetInteger("moving", 1);
            }
            else
            {
                gameObject.GetComponent<Animator>().SetInteger("moving", 0);
            }
        }
    }

    private List<Vector3> GetPath(List<TileBehaviour> tilesToVisit)
    {
        List<Vector3> result = new List<Vector3>();
        TileBehaviour beg = Field.getClosestTile(transform.position).GetComponent<TileBehaviour>();
        for (int i = 0; i < tilesToVisit.Count; ++i)
        {
            result.AddRange(pointsToVisit(beg, tilesToVisit[i]));
            beg = tilesToVisit[i];
        }
        return result;
    }

    private List<Vector3> pointsToVisit(TileBehaviour beg, TileBehaviour end)
    {
        List<Vector3> result = new List<Vector3>();
        Queue<TileBehaviour> processing = new Queue<TileBehaviour>();
        beg.rate = 0;
        processing.Enqueue(beg);
        while (processing.Count > 0)
        {
            var c = processing.Dequeue();
            c.isVisited = true;
            foreach (var n in c.neighbours)
            {
                if (!n.isVisited)
                {
                    if (n.rate > c.rate + 1) n.rate = c.rate + 1;
                    processing.Enqueue(n);
                }
            }
        }

        var e = end;
        result.Add(end.gameObject.transform.position);
        while( e != beg)
        {
            var min = int.MaxValue;
            TileBehaviour minTb = null;
            foreach(var n in e.neighbours)
            {
                if (n.rate < min)
                {
                    min = n.rate;
                    minTb = n;
                }
            }
            result.Add(minTb.gameObject.transform.position);
            e = minTb;
        }
        result.Reverse();

        foreach (var t in Field.tiles)
        {
            t.isVisited = false;
            t.rate = int.MaxValue;
        }
        return result.GetRange(1, result.Count - 1);
    }

    void PlanningUpdate()
    {

    }

    void PauseUpdate()
    {

    }

    public void OnStateSwitched(GameStates currentState, GameStates previousState)
    {
        UpdateFunction = gameStateUpdateMapping[currentState];
        if (currentState == GameStates.Planning)
        {
            tilesToVisit = new List<TileBehaviour>();
        }
        if (currentState == GameStates.Acting)
        {
            path = null;
        }
    }

    internal void MoveOnTile(GameObject go2)
    {
        TileBehaviour tb = go2.GetComponent<TileBehaviour>();
        if (tb != null)
        {
            tilesToVisit.Add(tb);
            Debug.Log("added " + go2.transform.position.ToString());
        }
    }
}
