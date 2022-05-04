using System.Collections.Generic;
using UnityEngine;

public class LocationNodeController : MonoBehaviour {

    [SerializeField] private TileData parentTile;
    [Space(10)]
    [SerializeField] private SpawnerNode spawnerNodePedestrian;
    [SerializeField] private SpawnerNode spawnerNodeVehicle;
    [SerializeField] private LocationNode destinationNodePedestrian;
    [SerializeField] private LocationNode destinationNodeVehicle;
    [Space(10)]
    [SerializeField] private GameObject nodeLocation;
    [Space(10)]
    [SerializeField] private LocationType locationType;

    void Awake() {
        LocationRegistration.AddToList(this);
    }

    public SpawnerNode GetSpawnerNodeVehicle() { return spawnerNodeVehicle; }
    public SpawnerNode GetSpawnerNodePedestrian() { return spawnerNodePedestrian; }
    public LocationNode GetDestinationNodeVehicle() { return destinationNodeVehicle; }
    public LocationNode GetDestinationNodePedestrian() { return destinationNodePedestrian; }

    public void ArriveAtDestination(VehicleAgent agent) { destinationNodeVehicle.ProcessNodeLogic(agent); }
    public void ApproachDestination(VehicleAgent agent) { destinationNodeVehicle.PrepareNodeLogic(agent); }
    public void ArriveAtDestination(PedestrianAgent agent) { destinationNodePedestrian.ProcessNodeLogic(agent); }
    public void ApproachDestination(PedestrianAgent agent) { destinationNodePedestrian.PrepareNodeLogic(agent); }

    public TileData GetParentTile() {
        if (parentTile == null) {
            Debug.LogError("Couldn't get parent tile! Object: " + gameObject.name + ", direct parent: " + gameObject.transform.parent.name);
        }

        return parentTile;
    }

    public GameObject GetNodeLocation() { //Needed for any tile bigger than 1x1 if the controller isn't on the destination tile
        if (nodeLocation != null) {
            return nodeLocation;
        }
        return gameObject;
    }

    public TilePos GetTilePosition() {
        return TilePos.GetTilePosFromLocation(transform.position);
    }

    public LocationType GetLocationType() {
        return locationType;
    }
}