using UnityEngine;

public class LocationNodeController : MonoBehaviour {

    [SerializeField] private GameObject spawnerNode;
    [SerializeField] private GameObject despawnerNode;
    [SerializeField] private GameObject startNode;
    [SerializeField] private GameObject destinationNode;

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
}