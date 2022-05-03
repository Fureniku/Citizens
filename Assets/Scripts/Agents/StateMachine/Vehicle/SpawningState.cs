using System;
using UnityEngine;

public class SpawningState : VehicleBaseState {
    
    public SpawningState(VehicleAgent agent) {
        this.stateName = "Spawning State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        agent.GetAgent().isStopped = true;
        if (agent.IsInitialized()) {
            return typeof(DriveState);
        }
        return null;
    }
    
    public override Type StateEnter() {
        agent.GetAgent().isStopped = true;
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}