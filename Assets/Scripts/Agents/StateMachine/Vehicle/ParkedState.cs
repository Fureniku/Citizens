using System;
using UnityEngine;
using UnityEngine.AI;

public class ParkedState : VehicleBaseState {
    
    private int waitForLeave = 0;
    
    public ParkedState(VehicleAgent agent) {
        this.stateName = "Parked State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (waitForLeave < 120) {
            waitForLeave++;
        } else {
            if (!agent.IsVacant()) {
                agent.EjectPassengers();
            }
        }
        
        return null;
    }
    public override Type StateEnter() {
        agent.GetAgent().isStopped = true;
        Vector3 rot = agent.gameObject.transform.eulerAngles;
        float yRot = Mathf.Round(rot.y/90) * 90;
        agent.gameObject.transform.rotation = Quaternion.Euler(rot.x, yRot, rot.z);
        agent.GetAgent().enabled = false;
        agent.GetComponent<NavMeshObstacle>().enabled = true;

        ParkingNode node = agent.GetCurrentDestination().GetComponent<ParkingNode>();
        if (node != null) {
            node.OccupySpace();
        }
        return null;
    }

    public override Type StateExit() {
        agent.GetComponent<NavMeshObstacle>().enabled = false;
        agent.GetAgent().enabled = true;
        return null;
    }
}