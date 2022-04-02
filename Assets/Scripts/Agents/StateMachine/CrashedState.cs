using System;

public class CrashedState : BaseState {

    public CrashedState(BaseAgent agent) {
        this.stateName = "Crashed State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        return null;
    }

    public override Type StateEnter() {
        agent.GetAgent().isStopped = true;
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}