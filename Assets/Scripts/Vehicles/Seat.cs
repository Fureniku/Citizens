using UnityEngine;

public class Seat : MonoBehaviour {

    [SerializeField] private Vector3 exitPos;
    [SerializeField] private bool occupied;
    [SerializeField] private GameObject occupyingAgent;

    public void SetAgentOccupancy(GameObject agent) {
        occupyingAgent = agent;
        occupied = true;
    }

    public bool IsAvailable() {
        return !occupied;
    }

    public void ExitSeat() {
        occupyingAgent.transform.position = GetExitPos();
        occupyingAgent.GetComponent<PedestrianAgent>().ExitVehicle();
        occupyingAgent = null;
        occupied = false;
    }

    public Vector3 GetExitPos() {
        return transform.position + transform.parent.rotation * exitPos;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawLine(transform.position, GetExitPos());
        Gizmos.DrawCube(GetExitPos(), new Vector3(0.2f, 0.2f, 0.2f));
    }
}