using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWorld : MonoBehaviour {

    [SerializeField] private GameObject tile;
    [SerializeField] private VehicleRegistry vehicleRegistry;
    private float roadScale = 0.75f;

    // Start is called before the first frame update
    void Start() {
        vehicleRegistry.Initialize();

        
        float tilePos = 0;
        
        for (int i = 0; i < VehicleRegistry.GetTotalCars(); i++) {
            if (i % 2 == 0) {
                GameObject placedTile = Instantiate(tile, new Vector3(tilePos, 0, 0), Quaternion.Euler(0, 90, 0));
                placedTile.transform.localScale = new Vector3(roadScale, roadScale, roadScale);
            }

            GameObject vehicle = Instantiate(VehicleRegistry.GetCar(i), new Vector3(tilePos, 0.19f, 1.2f), Quaternion.Euler(0, 90, 0));
            vehicle.GetComponent<VehicleAgent>().enabled = false;
            vehicle.GetComponent<AgentStateMachine>().enabled = false;
            
            tilePos += roadScale * 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
