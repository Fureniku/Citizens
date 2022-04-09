using System;

public abstract class BaseState {
    
    public abstract Type StateEnter();
    public abstract Type StateExit();
}