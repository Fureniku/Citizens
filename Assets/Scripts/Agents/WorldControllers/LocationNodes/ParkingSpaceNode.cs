using UnityEngine;

public class ParkingSpaceNode : MonoBehaviour {

    [SerializeField] private bool occupied;

    public bool IsOccupied() {
        return occupied;
    }

    public void OccupySpace() => occupied = true;
    public void LeaveSpace() => occupied = false;
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;


        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }

}
