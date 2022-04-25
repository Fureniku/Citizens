using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class DespawningState : VehicleBaseState {

    public DespawningState(VehicleAgent agent) {
        this.stateName = "Despawning State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        float distance = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);

        if (distance < 1) {
            Object.Destroy(agent.gameObject);
        }
        
        return null;
    }
    
    public override Type StateEnter() {
        Debug.Log("Entering despawn state");
        return null;
    }

    public override Type StateExit() {
        Debug.Log("exiting despawn state");
        return null;
    }
}