using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.AI;

public class TestAgentNoWorld : MonoBehaviour {
    
    private NavMeshAgent agent;
    //[SerializeField] private GameObject[] destinations;
    [SerializeField] private int currentDest = 0;

    [SerializeField] private bool shouldStop = false;
    [SerializeField] private int stopTime = 0;

    [SerializeField] private AStar aStar;

    private bool initialized = false;

    private List<GameObject> dests;
    
    [SerializeField] private GameObject finalDest;

    private float accelerationSave = 0; //Saved acceleration value used for delaying vehicle motion start.
    
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        dests = new List<GameObject>();
    }

    private void Init() {
        Debug.Log("Requesting path...");
        List<Node> path = aStar.RequestPath(gameObject, finalDest);
        Debug.Log("Generated path with " + path.Count + " nodes");
        initialized = true;

        for (int i = 0; i < path.Count; i++) {
            TileData td = World.Instance.GetChunkManager().GetTile(new TilePos(path[i].x, path[i].y));
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, path.Count)) entryTd = World.Instance.GetChunkManager().GetTile(new TilePos(path[i-1].x, path[i-1].y));
                if (NodeInRange(i + 1, path.Count)) exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(path[i+1].x, path[i+1].y));

                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetGridPos(), td.GetGridPos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetGridPos(), exitTd.GetGridPos()); //current to exit

                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    Debug.Log("Adding " + entryGo + " to destinations");
                    dests.Add(entryGo);
                    Debug.Log("Adding " + exitGo + " to destinations");
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
    }

    private bool NodeInRange(int node, int range) {
        return node > 0 && node < range;
    }

    void FixedUpdate() {
        if (!initialized) {
            Init();
        }
        
        float dist = Vector3.Distance(transform.position, dests[currentDest].transform.position);
        if (dist < 5) {
            if (currentDest < dests.Count-1) {
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
        agent.destination = dests[currentDest].transform.position;

        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
            stopTime = 0;
        }
    }
}
