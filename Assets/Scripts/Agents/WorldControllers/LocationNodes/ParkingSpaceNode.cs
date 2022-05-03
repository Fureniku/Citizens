using UnityEngine;

public class ParkingSpaceNode : MonoBehaviour {

    [SerializeField] private bool occupied;
    [SerializeField] private bool claimed;

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
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }

}
