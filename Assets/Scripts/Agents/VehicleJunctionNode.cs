using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleJunctionNode : MonoBehaviour {
    
    [SerializeField] private bool giveWay = false;
    [SerializeField] private bool isIn = true; // True for in (entering tile), false for out (exiting tile). 

    private TileRoad road;

    void Awake() {
        road = transform.parent.GetComponent<TileRoad>();
    }
    
    public bool GiveWay() {
        return giveWay;
    }

    public bool IsIn() {
        return isIn;
    }

    public bool IsOut() {
        return !isIn;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = isIn ? Color.green : Color.red;

        Gizmos.DrawSphere(transform.position, 1.0f);
        
    }
}
