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

    FieldBehaviour Field = null;

    public float speed = 1f;

    public Plan pawnPlan = new Plan();

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

    void ActUpdate()
    {
        pawnPlan.Execute();
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
            pawnPlan = new Plan();
        }
        if (currentState == GameStates.Acting)
        {

        }
    }

    internal void MoveOnTile(GameObject go2)
    {
        TileBehaviour tb = go2.GetComponent<TileBehaviour>();
        if (tb != null)
        {
            pawnPlan.AddCommand(new MoveCommand(tb, gameObject, speed));
            Debug.Log("added " + go2.transform.position.ToString());
        }
    }
}
