using System;

public abstract class AgentBaseState : BaseState {
    
    protected BaseAgent agent;
    protected string stateName;
    protected bool waitableState = false;

    public string GetName() {
        return stateName;
    }

    public BaseAgent GetAgent() {
        return agent;
    }

    public bool IsWaitableState() {
        return waitableState;
    }
    
    public abstract Type StateUpdate();
}