using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Loading.States;
using UnityEngine;

public class LoadStateMachine : MonoBehaviour {
    
    private Dictionary<Type, LoadBaseState> states;
    private LoadBaseState currentState;
    [SerializeField] private string stateName;

    public LoadBaseState CurrentState {
        get { return currentState; }
        set { currentState = value; }
    }

    public void SetStates(Dictionary<Type, LoadBaseState> states) {
        this.states = states;
    }

    public Dictionary<Type, LoadBaseState> GetStates() {
        return states;
    }

    // Update is called once per frame
    void FixedUpdate() {

        if (CurrentState == null) {
            CurrentState = states.Values.First();
            CurrentState.StateEnter();
        }
        if (CurrentState.GetType() == typeof(CompletedLoadState)) { //Dont do anything once state machine is finished
            return;
        }
        
        if (CurrentState.ShouldSkip() || CurrentState.StateProgress()) {
            SwitchToState(CurrentState.GetNextState());
        }
        
        stateName = currentState.GetName();
    }

    //Switch states, and update rule based system
    public void SwitchToState(Type nextState) {
        Debug.Log("MOVING FROM STATE [" + CurrentState.GetName() + "] TO [" + states[nextState].GetName() + "].");
        if (!CurrentState.ShouldSkip()) CurrentState.StateExit();
        CurrentState = states[nextState];
        if (!CurrentState.ShouldSkip()) CurrentState.StateEnter();
    }
}