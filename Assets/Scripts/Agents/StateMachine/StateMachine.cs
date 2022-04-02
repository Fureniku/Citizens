using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour {
    
    private Dictionary<Type, BaseState> states;
    private BaseState currentState;
    private BaseState lastState;
    private bool debug = false; //State switch voicelines can be a bit spammy

    public BaseState CurrentState {
        get { return currentState; }
        set { currentState = value; }
    }

    public void SetStates(Dictionary<Type, BaseState> states) {
        this.states = states;
    }

    public Dictionary<Type, BaseState> GetStates() {
        return states;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (CurrentState == null) {
            CurrentState = states.Values.First();
            CurrentState.StateEnter();
        }
        else {
            Type nextState = CurrentState.StateUpdate();
            
            if (nextState != null && nextState != CurrentState.GetType()) {
                SwitchToState(nextState);
            }
        }
    }

    //Switch states, and update rule based system
    public void SwitchToState(Type nextState) {
        CurrentState.StateExit();
        lastState = CurrentState;
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }

    public BaseState LastState() {
        return lastState;
    }
}