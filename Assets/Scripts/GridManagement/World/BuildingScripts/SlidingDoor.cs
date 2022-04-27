using UnityEngine;

public class SlidingDoor : Door {

	[SerializeField] private Vector3 closedPosition;
	[SerializeField] private Vector3 openPosition;

	[SerializeField] private float moveSpeed = 10.0f;

	private Vector3 openPos;
	private Vector3 closedPos;

	void Start() {
		openPos = GetOffsetPosition(openPosition);
		closedPos = GetOffsetPosition(closedPosition);
	}
	
	public int interpolationFramesCount = 45; // Number of frames to completely interpolate between the 2 positions
	int elapsedFrames = 0;
	
	void Update() {
		
		float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
		
		if (shouldOpen) {
			transform.position = Vector3.Lerp(closedPos, openPos, interpolationRatio);
		} else {
			transform.position = Vector3.Lerp(openPos, closedPos, interpolationRatio);
		}

		if (elapsedFrames < interpolationFramesCount) {
			elapsedFrames++;
		}

		if (openLastFrame != shouldOpen) {
			openLastFrame = shouldOpen;
			elapsedFrames = 0;
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(closedPos, 0.5f);
		Gizmos.DrawLine(closedPos, openPos);
		Gizmos.DrawCube(openPos, new Vector3(0.5f, 0.5f, 0.5f));
	}
}
