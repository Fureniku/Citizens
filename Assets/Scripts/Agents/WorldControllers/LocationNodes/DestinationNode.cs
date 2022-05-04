using UnityEngine;

public class DestinationNode : LocationNode {

    [SerializeField] private DespawnerNode despawnerNode;
    [SerializeField] private DestinationAction destinationAction;

    public override void ProcessNodeLogic(BaseAgent agent) {
        switch (destinationAction) {
            case DestinationAction.DESTROY:
                Destroy(agent.gameObject);
                break;
            case DestinationAction.MOVE_TO_DESTROY:
                if (agent is VehicleAgent) {
                    agent.IncrementDestination();
                    agent.GetStateMachine().ForceState(typeof(DespawningState));
                } else {
                    agent.ForceAgentDestination(despawnerNode.gameObject);
                }
                break;
            case DestinationAction.ASK_AGENT:
                agent.OnArrival();
                break;
            default:
                Debug.LogWarning("Destination type " + destinationAction + " not implemented.");
                break;
        }
    }
    
    public override void PrepareNodeLogic(BaseAgent agent) {
        if (destinationAction == DestinationAction.MOVE_TO_DESTROY) {
            agent.AddNewDestination(despawnerNode.gameObject);
        }
    }
}

public enum DestinationAction {
    DESTROY,
    MOVE_TO_DESTROY,
    MOVE_TO_NEXT_DESTINATION,
    MOVE_TO_RANDOM_DESTINATION,
    IDLE,
    ASK_AGENT
}