using UnityEngine;

public class LocationNode : MonoBehaviour {

    [SerializeField] private NodeType nodeType = NodeType.START;
    [SerializeField] private LocationNodeController nodeController;

    public NodeType GetNodeType() {
        return nodeType;
    }

    public LocationNodeController GetNodeController() {
        return nodeController.GetComponent<LocationNodeController>();
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        if (nodeType == NodeType.START || nodeType == NodeType.SPAWNER) {
            Gizmos.color = Color.blue;
        }
        if (nodeType == NodeType.DESTINATION || nodeType == NodeType.DESPAWNER) {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }
}

public enum NodeType {
    START, //Vehicles can start here but aren't created here. Can be targetted from spawner
    DESTINATION, //Vehicles end here but aren't destroyed. Can forward onto start or despawner.
    SPAWNER, //Vehicles can be spanwed here
    DESPAWNER //Vehicles despawn here
}