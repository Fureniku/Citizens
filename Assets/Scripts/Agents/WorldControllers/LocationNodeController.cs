using System.Collections.Generic;
using UnityEngine;

public class LocationNodeController : MonoBehaviour {

    [SerializeField] private TileData parentTile;
    [Space(10)]
    [SerializeField] private LocationNode spawnerNode;
    [SerializeField] private LocationNode despawnerNode;
    [SerializeField] private LocationNode startNode;
    [SerializeField] private LocationNode destinationNode;
    [Space(10)]
    [SerializeField] private ParkingController parkingController;
    [Space(10)]
    [SerializeField] private DestinationAction destinationAction;
    [SerializeField] private LocationType locationType;

    void Awake() {
        DestinationRegistration.AddToList(this);
    }

    public GameObject GetSpawnerNode() {
        return spawnerNode.gameObject;
    }
    
    public GameObject GetDespawnerNode() {
        return despawnerNode.gameObject;
    }
    
    public GameObject GetStartNode() {
        return startNode.gameObject;
    }
    
    public GameObject GetDestinationNode() {
        return destinationNode.gameObject;
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
                agent.SetAgentDestruction(despawnerNode.gameObject);
                break;
            case DestinationAction.GO_TO_PARKING:
                if (agent is VehicleAgent) {
                    ((VehicleAgent) agent).SetAgentParking(parkingController.GetFirstAvailableSpace().gameObject);
                }
                break;
        }
    }

    public void ArriveAtDestruction(BaseAgent agent) {
        agent.PrintText("Arriving at destruction node! Action is " + destinationAction);
        Destroy(agent.gameObject);
    }

    public TileData GetParentTile() {
        return parentTile;
    }

    public TilePos GetTilePosition() {
        return TilePos.GetTilePosFromLocation(transform.position);
    }

    public LocationType GetLocationType() {
        return locationType;
    }

    public ParkingController GetParkingController() {
        return parkingController;
    }
}

public enum DestinationAction {
    DESTROY_IMMEDIETE,
    GO_TO_DESTRUCTION_NODE,
    GO_TO_PARKING
}