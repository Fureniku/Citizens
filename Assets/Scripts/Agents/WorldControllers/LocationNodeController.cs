using UnityEngine;

public class LocationNodeController : MonoBehaviour {

    [SerializeField] private GameObject spawnerNode;
    [SerializeField] private GameObject despawnerNode;
    [SerializeField] private GameObject startNode;
    [SerializeField] private GameObject destinationNode;

    [SerializeField] private DestinationAction destinationAction;

    public GameObject GetSpawnerNode() {
        return spawnerNode;
    }
    
    public GameObject GetDespawnerNode() {
        return despawnerNode;
    }
    
    public GameObject GetStartNode() {
        return startNode;
    }
    
    public GameObject GetDestinationNode() {
        return destinationNode;
    }

    public bool CanDestroyAfterDestination() {
        return destinationNode != null && despawnerNode != null;
    }

    public void ArriveAtDestination(BaseAgent agent) {
        agent.PrintText("Arriving at destination node! Action is " + destinationAction);
        switch (destinationAction) {
            case DestinationAction.DESTROY_IMMEDIETE:
                Destroy(agent.gameObject);
                break;
            case DestinationAction.GO_TO_DESTRUCTION_NODE:
                agent.SetAgentDestruction(despawnerNode);
                break;
            case DestinationAction.GO_TO_PARKING:
                //todo parking stuff
                break;
        }
    }

    public void ArriveAtDestruction(BaseAgent agent) {
        agent.PrintText("Arriving at destruction node! Action is " + destinationAction);
        Destroy(agent.gameObject);
    }
}

public enum DestinationAction {
    DESTROY_IMMEDIETE,
    GO_TO_DESTRUCTION_NODE,
    GO_TO_PARKING
}