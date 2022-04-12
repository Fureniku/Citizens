using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentStateMachine : MonoBehaviour {
    
    private Dictionary<Type, AgentBaseState> states;
    private AgentBaseState currentState;
    private AgentBaseState lastState;

    public AgentBaseState CurrentState {
        get { return currentState; }
        set { currentState = value; }
    }

    public void SetStates(Dictionary<Type, AgentBaseState> states) {
        this.states = states;
    }

    public Dictionary<Type, AgentBaseState> GetStates() {
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

    public AgentBaseState LastState() {
        return lastState;
    }
}