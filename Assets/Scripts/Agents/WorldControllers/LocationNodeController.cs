using System.Collections.Generic;
using UnityEngine;

public class LocationNodeController : MonoBehaviour {

    [SerializeField] private TileData parentTile;
    [Space(10)]
    [SerializeField] private LocationNode spawnerNode;
    [SerializeField] private LocationNode startNode;
    [SerializeField] private LocationNode destinationNode;
    [Space(10)]
    [SerializeField] private LocationType locationType;

    void Awake() {
        DestinationRegistration.AddToList(this);
    }

    public GameObject GetSpawnerNode() {
        return spawnerNode.gameObject;
    }

    public GameObject GetStartNode() {
        return startNode.gameObject;
    }
    
    public LocationNode GetDestinationNode() {
        return destinationNode;
    }

    public void ArriveAtDestination(BaseAgent agent) {
        destinationNode.ProcessNodeLogic(agent);
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