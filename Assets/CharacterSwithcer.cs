using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IPawnAssetSelectorListener
{
    void OnPawnAssetSelect(GameObject pawn);
    GameObject PawnAsset { get; set; }
}
public class CharacterSwithcer : MonoBehaviour, IGameStateReacting
{
    public List<GameObject> CharacterAssets = new List<GameObject>();
    // Start is called before the first frame update
    public CharacterSelectEvent CharacterSelectEvent = new CharacterSelectEvent();
    // Update is called once per frame
    Action CurrentUpdate;

    public Dictionary<GameStates, Action> gameStateUpdateMapping { get; set; }

    public void OnStateSwitched(GameStates currentState, GameStates previousState)
    {
        if (currentState == GameStates.Planning)
        {
            CurrentUpdate = PlanningUpdate;
        }
        else
        {
            CurrentUpdate = null;
        }
    }

    void Update()
    {
        CurrentUpdate?.Invoke();
    }

    void PlanningUpdate()
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

    
}
