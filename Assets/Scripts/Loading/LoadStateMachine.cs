using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadStateMachine : MonoBehaviour {
    
    private Dictionary<Type, LoadBaseState> states;
    private LoadBaseState currentState;
    private bool debug = false; //State switch voicelines can be a bit spammy

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
        else {
            if (CurrentState.StateProgress()) {
                SwitchToState(CurrentState.GetNextState());
            }
        }
    }

    //Switch states, and update rule based system
    public void SwitchToState(Type nextState) {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }
}