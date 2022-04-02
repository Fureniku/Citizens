using System;

public abstract class BaseState {
    
    protected BaseAgent agent;
    protected string stateName;

    public string GetName() {
        return stateName;
    }

    public BaseAgent GetAgent() {
        return agent;
    }

    public abstract Type StateUpdate();
    public abstract Type StateEnter();
    public abstract Type StateExit();
}