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

    [SerializeField] private bool destroyOnArrival = false;
    [SerializeField] private bool shouldStop = false;
    [SerializeField] private int stopTime = 0;

    private AStar aStar;

    private bool initialized = false;

    private List<GameObject> dests;
    [SerializeField, ReadOnly] private int totalDestinations;
    
    //[SerializeField] private GameObject finalDest;
    
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
                        ReachedDestination();
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

    private void ReachedDestination() {
        if (destroyOnArrival) {
            Destroy(gameObject);
        }
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
        GameObject finalDest = World.Instance.GetGridManager().GetTile(DestinationRegistration.RoadDestinationRegistry.GetAtRandom()).gameObject;
        
        
        List<Node> path = aStar.RequestPath(gameObject, finalDest);

        if (path.Count > 2) {
            TileData tdStart = World.Instance.GetGridManager().GetTile(new TilePos(path[0].x, path[0].y));
            TileData tdNext = World.Instance.GetGridManager().GetTile(new TilePos(path[1].x, path[1].y));

            EnumDirection startingDirection = Direction.GetDirectionOffset(tdStart.GetGridPos(), tdNext.GetGridPos());

            if (tdStart.GetTile() == TileRegistry.STRAIGHT_ROAD_1x1) {
                Debug.Log("Offsetting " + gameObject.name + " for direction " + startingDirection);
                Debug.Log("Startingt at " + transform.position);
                switch (startingDirection) {
                    case EnumDirection.NORTH:
                        transform.position += new Vector3(-10f, 0, 0);
                        break;
                    case EnumDirection.EAST:
                        transform.position += new Vector3(0, 0, 10f);
                        break;
                    case EnumDirection.SOUTH:
                        transform.position += new Vector3(10f, 0, 0);
                        break;
                    case EnumDirection.WEST:
                        transform.position += new Vector3(0, 0, -10f);
                        break;
                }

                agent.Warp(transform.position);
                Debug.Log("Now at " + transform.position);
            }
        }

        for (int i = 0; i < path.Count; i++) {
            TileData td = World.Instance.GetGridManager().GetTile(new TilePos(path[i].x, path[i].y));
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, path.Count)) entryTd = World.Instance.GetGridManager().GetTile(new TilePos(path[i-1].x, path[i-1].y));
                if (NodeInRange(i + 1, path.Count)) exitTd = World.Instance.GetGridManager().GetTile(new TilePos(path[i+1].x, path[i+1].y));

                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetGridPos(), td.GetGridPos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetGridPos(), exitTd.GetGridPos()); //current to exit

                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    dests.Add(entryGo);
                    dests.Add(exitGo);
                }
            }
        }

        if (finalDest.GetComponent<LocationNodeController>() != null) {
            LocationNodeController lnc = finalDest.GetComponent<LocationNodeController>();

            if (lnc.CanDestroyAfterDestination()) {
                Debug.Log("Destination and destruction");
                dests.Add(lnc.GetDestinationNode());
                dests.Add(lnc.GetDespawnerNode());
                destroyOnArrival = true;
            }
            else if (lnc.GetDestinationNode() != null) {
                Debug.Log("Destination only");
                dests.Add(lnc.GetDestinationNode());
            }
            else if (lnc.GetDespawnerNode() != null) {
                Debug.Log("Destruction only");
                dests.Add(lnc.GetDespawnerNode());
                destroyOnArrival = true;
            }
            else {
                Debug.Log("Node controller with no node");
                dests.Add(finalDest);
            }
        }
        else {
            Debug.Log("No node controller");
            dests.Add(finalDest);
        }
        
        
        
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
