using System;
using UnityEngine;

public class ParkingState : VehicleBaseState {
    
    private ParkingController parkingController;
    
    public ParkingState(VehicleAgent agent) {
        this.stateName = "Parking State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
        
        if (dist < 5) {
            ParkingNode node = agent.GetCurrentDestination().GetComponent<ParkingNode>();
            if (node != null && node.IsOccupied()) {
                agent.SetAgentDestination(parkingController.GetFirstAvailableSpace().gameObject);
            }
        }
        
        if (dist < 0.5f) {
            return typeof(ParkedState);
        }
        return null;
    }
    public override Type StateEnter() {
        agent.SetSpeed(7.5f);
        parkingController = agent.GetDestinationController().GetParkingController();
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}
