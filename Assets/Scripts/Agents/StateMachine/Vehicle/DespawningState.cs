using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class DespawningState : VehicleBaseState {
    
    private int timer = 0;

    public DespawningState(VehicleAgent agent) {
        this.stateName = "Despawning State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        float distance = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);

        if (distance < 1) {
            agent.GetAgentManager().RemoveAgent(agent.gameObject);
        }

        timer++;

        if (timer > 600) {
            agent.GetAgentManager().RemoveAgent(agent.gameObject);
        }
        
        return null;
    }
    
    public override Type StateEnter() {
        agent.SetSpeed(1000);
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}