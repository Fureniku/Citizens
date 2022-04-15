using UnityEngine;

public class CrossingController : MonoBehaviour {

    [SerializeField] private GameObject crossing1;
    [SerializeField] private GameObject crossing2;

    public GameObject GetClosestCrossing(Vector3 gameObjectIn) {
        float dist1 = Vector3.Distance(gameObjectIn, crossing1.transform.position);
        float dist2 = Vector3.Distance(gameObjectIn, crossing2.transform.position);

        if (dist1 < dist2) {
            return crossing1;
        }
        return crossing2;
    }

    public GameObject GetFurthestCrossing(Vector3 gameObjectIn) {
        float dist1 = Vector3.Distance(gameObjectIn, crossing1.transform.position);
        float dist2 = Vector3.Distance(gameObjectIn, crossing2.transform.position);

        if (dist1 < dist2) {
            return crossing2;
        }
        return crossing1;
    }
}