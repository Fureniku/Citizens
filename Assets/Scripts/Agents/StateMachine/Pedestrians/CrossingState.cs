using System;
using UnityEngine;

public class CrossingState : PedestrianBaseState {

    public CrossingState(PedestrianAgent agent) {
        this.stateName = "Crossing State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        if (!agent.IsOnRoad()) {
            return agent.GetPreviousState();
        }
        
        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}