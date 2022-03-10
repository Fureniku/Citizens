using System;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour {

    private NavMeshAgent agent;
    [SerializeField] private TilePos destination = new TilePos(0, 0);
    private bool isDestinationSet = false;

    private float accelerationSave = 0; //Saved acceleration value used for delaying vehicle motion start.

    public bool ready = false;
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToLocation() {
        destination = DestinationRegistration.RoadDestinationRegistry.GetAtRandom();
        Vector3 dest = TilePos.GetWorldPosFromTilePos(destination);
        
        agent.destination = dest;
        agent.isStopped = false;
        ready = true;
    }

    public void SaveAcceleration(float acc) {
        accelerationSave = acc;
        GetComponent<NavMeshAgent>().acceleration = 0;
    }
    
    public void RestoreAcceleration() => GetComponent<NavMeshAgent>().acceleration = accelerationSave;

    public bool IsAgentReady() {
        //Debug.Log("Checking if testagent is ready. Ready state: " + ready + ", path state: " + GetComponent<NavMeshAgent>().pathStatus);
        return ready && GetComponent<NavMeshAgent>().hasPath;
    }

    public NavMeshAgent GetAgent() {
        return GetComponent<NavMeshAgent>();
    }

    /*private void Update() {
        if (agent != null && agent.remainingDistance < 5f && TilePos.GetWorldPosFromTilePos(destination) != transform.position) {
            Debug.Log("agent has reached destination (" + agent.remainingDistance + " away)");
            //Destroy(gameObject);
        }
    }*/
}
