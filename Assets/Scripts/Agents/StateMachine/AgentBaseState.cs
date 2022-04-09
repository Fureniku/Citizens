using System;

public abstract class AgentBaseState : BaseState {
    
    protected BaseAgent agent;
    protected string stateName;

    public string GetName() {
        return stateName;
    }

    public BaseAgent GetAgent() {
        return agent;
    }
    
    public abstract Type StateUpdate();
}