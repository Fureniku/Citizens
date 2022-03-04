using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour {

    private NavMeshAgent agent;
    [SerializeField] private TilePos destination = new TilePos(0, 0);
    private bool isDestinationSet = false;

    public bool ready = false;
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToLocation() {
        Debug.Log("Navigating!");

        //Vector3 dest = TilePos.GetTrueWorldPos(World.Instance.GetGridManager().GetTile(destination).gameObject);
        destination = DestinationRegistration.RoadRegistry.GetAtRandom();
        Vector3 dest = TilePos.GetWorldPosFromTilePos(destination);
        
        agent.destination = dest;
        agent.isStopped = false;
        ready = true;
    }
}
