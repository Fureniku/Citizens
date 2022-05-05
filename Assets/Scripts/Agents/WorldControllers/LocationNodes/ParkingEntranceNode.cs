using UnityEngine;

public class ParkingEntranceNode : LocationNode {
    
    [SerializeField] private ParkingController parkingController;

    public ParkingController GetParkingController() {
        return parkingController; 
    }

    public override void ProcessNodeLogic(BaseAgent agent) {
        if (parkingController != null) {
            //agent.SetAgentDestination(parkingController.GetFirstAvailableSpace().gameObject);
            agent.GetStateMachine().ForceState(typeof(ParkingState));
        }
    }
    
    public override void PrepareNodeLogic(BaseAgent agent) {
        if (parkingController != null) {
            if (parkingController.GetFirstAvailableSpace() != null) {
                agent.AddNewDestination(parkingController.GetFirstAvailableSpace().gameObject);
            } else {
                agent.GetAgentManager().RemoveAgent(agent.gameObject); //Delete agent if car park is full. Not nice but time constraints :( 
            }
        }
    }
}