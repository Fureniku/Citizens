using System.Collections.Generic;
using UnityEngine;

public class ParkingController : MonoBehaviour {

    [SerializeField] private List<ParkingNode> parkingNodes = new List<ParkingNode>();
    [SerializeField] private GameObject agentDestination = null;

    void Awake() {
        parkingNodes.Clear();
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<ParkingNode>() != null) {
                parkingNodes.Add(child.GetComponent<ParkingNode>());
            }
        }
        
        Debug.Log("Parking controller registered with " + parkingNodes.Count + " nodes.");
    }
    
    public ParkingNode GetFirstAvailableSpace() {
        for (int i = 0; i < parkingNodes.Count; i++) {
            if (!parkingNodes[i].IsOccupied()) {
                return parkingNodes[i];
            }
        }

        return null; //car park is full :(
    }

    public GameObject GetForwardingAgentDestination() {
        return agentDestination;
    }
}