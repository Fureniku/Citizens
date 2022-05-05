using System;
using UnityEngine;

public class WalkState : PedestrianBaseState {

    public WalkState(PedestrianAgent agent) {
        this.stateName = "Walking State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.GetVehicle() != null) {
            return typeof(VehiclePassengerState);
        }

        agent.SetLookDirection();

        if (Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position) < 3.0f) {
            if (agent.GetDestinationController() != null) {
                if (agent.GetCurrentDestination().GetComponent<DespawnerNode>() != null) {
                    agent.GetAgentManager().RemoveAgent(agent.gameObject);
                }
                agent.GetDestinationController().ArriveAtDestination(agent);
            }
        }

        return EnteredRoad();
    }
    
    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}