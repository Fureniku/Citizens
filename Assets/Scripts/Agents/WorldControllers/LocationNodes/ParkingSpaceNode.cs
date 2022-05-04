using Tiles.TileManagement;
using UnityEngine;

public class ParkingSpaceNode : MonoBehaviour {

    [SerializeField] private bool occupied;
    [SerializeField] private bool claimed;

    [SerializeField] private EnumLocalDirection direction;

    public bool IsOccupied() {
        return occupied;
    }
    
    public bool IsClaimed() {
        return claimed;
    }

    public void OccupySpace() => occupied = true;
    public void ClaimSpace() => claimed = true;

    public void LeaveSpace() {
        occupied = false;
        claimed = false;
    }

    public EnumLocalDirection GetRotation() {
        return direction;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }

    public ParkingController GetParkingController() {
        return transform.parent.GetComponent<ParkingController>();
    }
}
