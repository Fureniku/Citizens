using System.Collections.Generic;
using UnityEngine;

public class LocationNodeController : MonoBehaviour {

    [SerializeField] private TileData parentTile;
    [Space(10)]
    [SerializeField] private LocationNode spawnerNode;
    [SerializeField] private LocationNode destinationNode;
    [Space(10)]
    [SerializeField] private LocationType locationType;

    void Awake() {
        LocationRegistration.AddToList(this);
    }

    public GameObject GetSpawnerNode() {
        return spawnerNode.gameObject;
    }

    public LocationNode GetDestinationNode() {
        return destinationNode;
    }

    public void ArriveAtDestination(BaseAgent agent) {
        destinationNode.ProcessNodeLogic(agent);
    }

    public void ApproachDestination(BaseAgent agent) {
        destinationNode.PrepareNodeLogic(agent);
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
}