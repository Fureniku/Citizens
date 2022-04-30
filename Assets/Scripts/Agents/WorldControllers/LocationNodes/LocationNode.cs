using UnityEngine;

public abstract class LocationNode : MonoBehaviour {

    [SerializeField] private LocationNodeController nodeController;
    [SerializeField] private Color gizmoCol;
    
    public LocationNodeController GetNodeController() {
        return nodeController.GetComponent<LocationNodeController>();
    }

    public abstract void ProcessNodeLogic(BaseAgent agent);
    public abstract void PrepareNodeLogic(BaseAgent agent);
    
    private void OnDrawGizmos() {
        Gizmos.color = gizmoCol;
        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }
}