using System.Collections.Generic;
using UnityEngine;

public class ParkingController : MonoBehaviour {

    [SerializeField] private List<ParkingSpaceNode> parkingNodes = new List<ParkingSpaceNode>();
    [SerializeField] private LocationNodeController locationNodeController;
    [SerializeField] private GameObject agentDestination = null;

    void Awake() {
        parkingNodes.Clear();
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<ParkingSpaceNode>() != null) {
                if (!locationNodeController.GetParentTile().IsRegistryVersion()) {
                    parkingNodes.Add(child.GetComponent<ParkingSpaceNode>());
                    LocationRegistration.carParkSpaces.Add(child.GetComponent<ParkingSpaceNode>());
                }
            }
        }
        
        Debug.Log("Parking controller registered with " + parkingNodes.Count + " nodes.");
    }
    
    public ParkingSpaceNode GetFirstAvailableSpace() {
        for (int i = 0; i < parkingNodes.Count; i++) {
            if (!parkingNodes[i].IsOccupied() && !parkingNodes[i].IsClaimed()) {
                parkingNodes[i].ClaimSpace();
                return parkingNodes[i];
            }
        }

        return null; //car park is full :(
    }

    public GameObject GetForwardingAgentDestination() {
        return agentDestination;
    }

    public LocationNodeController GetLocationNodeController() {
        return locationNodeController;
    }
}