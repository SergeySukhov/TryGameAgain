using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    // Start is called before the first frame update
    void Execute();

    // Update is called once per frame
    bool IsComplete { get; }
}

public class MoveCommand : ICommand
{
    public bool IsComplete { get; private set; }

    private TileBehaviour _from;
    private TileBehaviour _to;
    private FieldBehaviour _field = null;
    private List<Vector3> path = null;
    private int curIdx = 0;
    private GameObject gameObject;
    float _speed = 0f;

    public MoveCommand(TileBehaviour to, GameObject source, float speed = 1)
    {
        _to = to;
        IsComplete = false;
        _field = GameObject.FindGameObjectWithTag("Field").GetComponent<FieldBehaviour>();
        gameObject = source;
        _speed = speed;
    }

    public void Execute()
    {
        if (IsComplete) return;
        if (path == null)
        {
            _from = _field.getClosestTile(gameObject.transform.position).GetComponent<TileBehaviour>();
            path = GetPath(new List<TileBehaviour>() { _from, _to } );
            curIdx = 0;
        }

        if (path.Count > 0)
        {
            var pathLength = _speed * Time.deltaTime;
            var curPos = gameObject.transform.position;
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
                IsComplete = true;
            }
        }
    }

    private List<Vector3> GetPath(List<TileBehaviour> tilesToVisit)
    {
        List<Vector3> result = new List<Vector3>();
        TileBehaviour beg = _field.getClosestTile(_from.gameObject.transform.position).GetComponent<TileBehaviour>();
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
        while (e != beg)
        {
            var min = int.MaxValue;
            TileBehaviour minTb = null;
            foreach (var n in e.neighbours)
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

        foreach (var t in _field.tiles)
        {
            t.isVisited = false;
            t.rate = int.MaxValue;
        }
        return result.GetRange(1, result.Count - 1);
    }
}
