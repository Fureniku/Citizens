using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAgentNoWorld : MonoBehaviour {
    
    private NavMeshAgent agent;
    [SerializeField] private GameObject[] destinations;
    [SerializeField] private int currentDest = 0;

    [SerializeField] private bool shouldStop = false;
    [SerializeField] private int stopTime = 0;

    private float accelerationSave = 0; //Saved acceleration value used for delaying vehicle motion start.
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = destinations[0].transform.position;
        
        VehicleJunctionNode node = destinations[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
            stopTime = 0;
        }
    }

    void FixedUpdate() {
            float dist = Vector3.Distance(transform.position, destinations[currentDest].transform.position);
            if (dist < 5) {
                if (currentDest < destinations.Length-1) {
                    if (shouldStop) {
                        if (stopTime < 90) {
                            stopTime++;
                        }
                        else {
                            IncrementDestination();
                        }
                    }
                    else {
                        IncrementDestination();
                    }
                }
                else {
                    Debug.Log("You have reached your destination");
                }
            }
        
    }

    void IncrementDestination() {
        currentDest++;
        agent.destination = destinations[currentDest].transform.position;

        VehicleJunctionNode node = destinations[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
            stopTime = 0;
        }
    }
}
