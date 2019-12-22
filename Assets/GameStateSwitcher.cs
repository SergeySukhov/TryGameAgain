using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum GameStates
{
    Planning,
    Acting,
    Pause,
}

public interface IGameStateReacting
{
    void OnStateSwitched(GameStates currentState, GameStates previousState);

    Dictionary<GameStates, Action> gameStateUpdateMapping { get; set; }
}


// <Previous state, current state>
[Serializable]
public class GameStateSwitchedEvent : UnityEvent<GameStates, GameStates> { }

public class GameStateSwitcher : MonoBehaviour, IGameStateReacting, IPawnListener
{
    // Start is called before the first frame update
    GameStates CurrentSate = GameStates.Pause;
    GameStates PreviousSate = GameStates.Pause;

    public GameStateSwitchedEvent StateSwitched = new GameStateSwitchedEvent();

    public int PawnsActingCount = 0;
    public int PawnsFinishedActing = 0;

    public 
    void Start()
    {
        gameStateUpdateMapping = new Dictionary<GameStates, Action>();
        gameStateUpdateMapping.Add(GameStates.Pause, PauseUpdate);
        gameStateUpdateMapping.Add(GameStates.Planning, PlanningUpdate);
        gameStateUpdateMapping.Add(GameStates.Acting, ActingUpdate);
        StateSwitched.Invoke(GameStates.Pause, GameStates.Planning);
    }

    // Update is called once per frame
    void Update()
    {
        gameStateUpdateMapping[CurrentSate]();
    }

    void PauseUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StateSwitched.Invoke(PreviousSate, CurrentSate);
        }
    }


    float currentTimeSpent = 0f;
    float timeForPlanning = 60f;

    public Dictionary<GameStates, Action> gameStateUpdateMapping { get; set; }

    void PlanningUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) || currentTimeSpent >= timeForPlanning)
        {
            StateSwitched.Invoke(GameStates.Acting, CurrentSate);
        }
        currentTimeSpent += Time.deltaTime;
    }

    void ActingUpdate()
    {
        foreach( var go in GameObject.FindGameObjectsWithTag("Pawn"))
        {
            if (!go.GetComponent<PawnBehaviour>().pawnPlan.IsComplete) return;
        }
        StateSwitched.Invoke(GameStates.Planning, CurrentSate);
    }

    public void OnStateSwitched(GameStates currentState, GameStates previousState)
    {
        Debug.Log("Cur = " + currentState + " Prev = " + previousState);
        CurrentSate = currentState;
        PreviousSate = previousState;
        if (currentState == GameStates.Acting)
        {
            PawnsActingCount = 0;
            PawnsFinishedActing = 0;
        }
        if (CurrentSate == GameStates.Planning && previousState != GameStates.Pause)
        {
            currentTimeSpent = 0f;
        }
    }

    public void OnPawnCreated(GameObject pawn)
    {
        
    }

    public void OnPawnDestroyed(GameObject pawn)
    {
        
    }

    public void OnPawnStartActing(GameObject pawn)
    {
        PawnsActingCount++;
    }

    public void OnPawnFinishedActing(GameObject pawn)
    {
        PawnsFinishedActing++;
    }
}
