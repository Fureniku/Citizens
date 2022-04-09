using System;

public abstract class LoadBaseState : BaseState {
    
    protected BaseAgent agent;
    protected Type nextState;
    protected string stateName;

    public string GetName() {
        return stateName;
    }

    public BaseAgent GetAgent() {
        return agent;
    }

    public Type GetNextState() {
        return nextState;
    }

    public abstract bool StateProgress();
}