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
        return parentTile;
    }

    public TilePos GetTilePosition() {
        return TilePos.GetTilePosFromLocation(transform.position);
    }

    public LocationType GetLocationType() {
        return locationType;
    }
}