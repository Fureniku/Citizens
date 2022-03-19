using System;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour {

    private NavMeshAgent agent;

    [SerializeField] private GameObject camPos = null;

    private float accelerationSave = 0; //Saved acceleration value used for delaying vehicle motion start.

    [SerializeField] private int currentDest = 0;
    [SerializeField] private GameObject currentDestGO;

    [SerializeField] private bool shouldStop = false;
    [SerializeField] private int stopTime = 0;

    private AStar aStar;

    private bool initialized = false;

    private List<GameObject> dests;
    [SerializeField, ReadOnly] private int totalDestinations;
    
    [SerializeField] private GameObject finalDest;
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        dests = new List<GameObject>();
        aStar = World.Instance.GetAStarPlane().GetComponent<AStar>();
    }

    void FixedUpdate() {
        totalDestinations = dests.Count;
        if (initialized) {
            if (dests[currentDest] != null) {
                currentDestGO = dests[currentDest];
                float dist = Vector3.Distance(transform.position, dests[currentDest].transform.position);
                if (dist < 5) {
                    if (currentDest < dests.Count - 1) {
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
                        //vehicle is at destination
                    }
                }
            }
            else {
                Debug.Log("Destination " + currentDest + " is null for " + gameObject.name);
            }
        }
    }

    //Save the initiial acceleration value and temporarily set to zero to stop vehicle moving until we're ready.
    public void SaveAcceleration(float acc) {
        accelerationSave = acc;
        GetComponent<NavMeshAgent>().acceleration = 0;
    }
    
    public void RestoreAcceleration() => GetComponent<NavMeshAgent>().acceleration = accelerationSave;

    public bool IsAgentReady() {
        return initialized && GetComponent<NavMeshAgent>().hasPath;
    }

    public NavMeshAgent GetAgent() {
        return GetComponent<NavMeshAgent>();
    }

    public GameObject GetCamPos() {
        return camPos;
    }

    //Initialize vehicle, setting node paths.
    public void Init() {
        finalDest = World.Instance.GetGridManager().GetTile(DestinationRegistration.RoadDestinationRegistry.GetAtRandom()).gameObject;
        Debug.Log("Requesting path...");
        List<Node> path = aStar.RequestPath(gameObject, finalDest);
        Debug.Log("Generated path with " + path.Count + " nodes");

        for (int i = 0; i < path.Count; i++) {
            TileData td = World.Instance.GetGridManager().GetTile(new TilePos(path[i].x, path[i].y));
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, path.Count)) entryTd = World.Instance.GetGridManager().GetTile(new TilePos(path[i-1].x, path[i-1].y));
                if (NodeInRange(i + 1, path.Count)) exitTd = World.Instance.GetGridManager().GetTile(new TilePos(path[i+1].x, path[i+1].y));

                Debug.Log("trying to add nodes from tilepos " + path[i].x + ", "+ path[i].y);
                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetGridPos(), td.GetGridPos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetGridPos(), exitTd.GetGridPos()); //current to exit

                    Debug.Log("Enter from " + entry + ", exit to " + exit);
                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    Debug.Log("Adding " + entryGo.name + " to destinations");
                    dests.Add(entryGo);
                    Debug.Log("Adding " + exitGo.name + " to destinations");
                    dests.Add(exitGo);
                }
            }
        }
        
        dests.Add(finalDest);
        
        Debug.Log("Generated final route with " + dests.Count + " nodes");
        
        agent.destination = dests[0].transform.position;
        
        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
            stopTime = 0;
        }
        initialized = true;
    }
    
    private bool NodeInRange(int node, int range) {
        return node > 0 && node < range;
    }
    
    void IncrementDestination() {
        currentDest++;
        agent.destination = dests[currentDest].transform.position;

        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
            stopTime = 0;
        }
    }
}
