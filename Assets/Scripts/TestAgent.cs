using System;
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
        destination = DestinationRegistration.RoadDestinationRegistry.GetAtRandom();
        Vector3 dest = TilePos.GetWorldPosFromTilePos(destination);
        
        agent.destination = dest;
        agent.isStopped = false;
        ready = true;
    }

    /*private void Update() {
        if (agent != null && agent.remainingDistance < 5f && TilePos.GetWorldPosFromTilePos(destination) != transform.position) {
            Debug.Log("agent has reached destination (" + agent.remainingDistance + " away)");
            //Destroy(gameObject);
        }
    }*/
}
