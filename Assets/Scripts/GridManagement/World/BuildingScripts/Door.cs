using UnityEngine;

public class Door : MonoBehaviour {
	
	[SerializeField] protected bool shouldOpen;
	protected bool openLastFrame;

	public void OpenDoor() => shouldOpen = true;
	public void CloseDoor() => shouldOpen = false;

	protected Vector3 GetOffsetPosition(Vector3 pos) {
		return transform.position + transform.parent.rotation * (pos * World.Instance.GetChunkManager().GetWorldScale());
	}
}