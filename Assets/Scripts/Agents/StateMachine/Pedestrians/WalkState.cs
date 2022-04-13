using System;

public class WalkState : AgentBaseState {

    public WalkState(BaseAgent agent) {
        this.stateName = "Walking State";
        this.agent = agent;
    }
    
    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }

    public override Type StateUpdate() {
        return null;
    }
}