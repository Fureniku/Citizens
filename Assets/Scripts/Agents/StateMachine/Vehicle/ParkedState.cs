using System;
using UnityEngine;

public class ParkedState : VehicleBaseState {
    
    public ParkedState(VehicleAgent agent) {
        this.stateName = "Parked State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        return null;
    }
    public override Type StateEnter() {
        agent.GetAgent().isStopped = true;
        Vector3 rot = agent.gameObject.transform.eulerAngles;
        float yRot = Mathf.Round(rot.y/90) * 90;
        agent.gameObject.transform.rotation = Quaternion.Euler(rot.x, yRot, rot.z);

        ParkingNode node = agent.GetCurrentDestination().GetComponent<ParkingNode>();
        if (node != null) {
            node.OccupySpace();
        }
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}