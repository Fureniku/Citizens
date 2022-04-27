using UnityEngine;

public class ParkingEntranceNode : LocationNode {
    
    [SerializeField] private ParkingController parkingController;

    public ParkingController GetParkingController() {
        return parkingController; 
    }

    public override void ProcessNodeLogic(BaseAgent agent) {
        if (parkingController != null) {
            agent.SetAgentDestination(parkingController.GetFirstAvailableSpace().gameObject);
            agent.GetStateMachine().ForceState(typeof(ParkingState));
        }
    }
}