using System;

public abstract class LoadBaseState : BaseState {
    
    protected Type nextState;
    protected string stateName;
    protected int progressId = 0;
    protected GenerationSystem system = null;

    public string GetName() {
        return stateName;
    }

    public Type GetNextState() {
        return nextState;
    }

    public int GetProgressId() {
        return progressId;
    }

    public GenerationSystem GetSystem() {
        return system;
    }
    
    public abstract bool StateProgress();
    public abstract string GetProgressString();
}