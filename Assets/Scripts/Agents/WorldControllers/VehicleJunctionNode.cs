using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleJunctionNode : MonoBehaviour {
    
    [SerializeField] private bool giveWay;
    [SerializeField] private bool isIn = true; // True for in (entering tile), false for out (exiting tile). 

    public void SetGiveWay(bool b) => giveWay = b;
    public void SetIsIn(bool b) => isIn = b;
    
    public bool GiveWay() {
        return giveWay;
    }

    public bool IsIn() {
        return isIn;
    }

    public bool IsOut() {
        return !isIn;
    }

    public VehicleJunctionController GetController() {
        return transform.parent.parent.GetComponent<VehicleJunctionController>(); //Parent of parent
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = isIn ? Color.green : Color.red;

        Gizmos.DrawSphere(transform.position, 1.0f * World.Instance.GetChunkManager().GetWorldScale());
        
    }
}
