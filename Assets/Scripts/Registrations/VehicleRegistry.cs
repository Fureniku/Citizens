using System.Collections.Generic;
using UnityEngine;

public class VehicleRegistry : MonoBehaviour {
    
    private static readonly GameObject[] registry = new GameObject[255];
    [SerializeField] private GameObject[] register = null;

    private static List<GameObject> cars = new List<GameObject>();
    private static List<GameObject> vans = new List<GameObject>();
    private static List<GameObject> trucks = new List<GameObject>();
    private static List<GameObject> busses = new List<GameObject>();

    [SerializeField] private Material mat_red = null;
    [SerializeField] private Material mat_blue = null;
    [SerializeField] private Material mat_yellow = null;
    [SerializeField] private Material mat_green = null;
    [SerializeField] private Material mat_silver = null;
    [SerializeField] private Material mat_black = null;
    [SerializeField] private Material mat_white = null;
    [SerializeField] private Material mat_orange = null;
    [SerializeField] private Material mat_purple = null;
    
    private static Material MAT_RED = null;
    private static Material MAT_BLUE = null;
    private static Material MAT_YELLOW = null;
    private static Material MAT_GREEN = null;
    private static Material MAT_SILVER = null;
    private static Material MAT_BLACK = null;
    private static Material MAT_WHITE = null;
    private static Material MAT_ORANGE = null;
    private static Material MAT_PURPLE = null;


    public void Initialize() {
        for (int i = 0; i < register.Length; i++) {
            GameObject reg = Instantiate(register[i], transform, true);
            reg.transform.position = new Vector3(0, -100, 0);
            reg.SetActive(false);
            registry[i] = reg;
        }
        
        PopulateRegistries();

        MAT_RED = mat_red;
        MAT_BLUE = mat_blue;
        MAT_YELLOW = mat_yellow;
        MAT_GREEN = mat_green;
        MAT_SILVER = mat_silver;
        MAT_BLACK = mat_black;
        MAT_WHITE = mat_white;
        MAT_ORANGE = mat_orange;
        MAT_PURPLE = mat_purple;
    }
    
    private void PopulateRegistries() {
        for (int i = 1; i < register.Length; i++) { //Skip entry zero (test car), only there coz zero based list messes with me :)
            GameObject go = register[i];
            if (go != null) {
                VehicleAgent vehicle = go.GetComponent<VehicleAgent>();
                if (vehicle != null) {
                    switch (vehicle.GetVehicleType()) {
                        case VehicleType.CAR:
                            cars.Add(go);
                            break;
                        case VehicleType.VAN:
                            vans.Add(go);
                            break;
                        case VehicleType.TRUCK:
                            trucks.Add(go);
                            break;
                        case VehicleType.BUS:
                            busses.Add(go);
                            break;
                    }
                }
            }
        }
        
        Debug.Log("Registered " + cars.Count + " cars.");
        Debug.Log("Registered " + vans.Count + " vans.");
        Debug.Log("Registered " + trucks.Count + " trucks.");
        Debug.Log("Registered " + busses.Count + " busses.");
    }

    public static Material GetMaterial(VehicleColour colour) {
        switch (colour) {
            case VehicleColour.RED:
                return MAT_RED;
            case VehicleColour.BLUE:
                return MAT_BLUE;
            case VehicleColour.YELLOW:
                return MAT_YELLOW;
            case VehicleColour.GREEN:
                return MAT_GREEN;
            case VehicleColour.SILVER:
                return MAT_SILVER;
            case VehicleColour.BLACK:
                return MAT_BLACK;
            case VehicleColour.WHITE:
                return MAT_WHITE;
            case VehicleColour.ORANGE:
                return MAT_ORANGE;
            case VehicleColour.PURPLE:
                return MAT_PURPLE;
        }

        return MAT_WHITE;
    }
    
    public Material GetMaterialNonStatic(VehicleColour colour) {
        switch (colour) {
            case VehicleColour.RED:
                return mat_red;
            case VehicleColour.BLUE:
                return mat_blue;
            case VehicleColour.YELLOW:
                return mat_yellow;
            case VehicleColour.GREEN:
                return mat_green;
            case VehicleColour.SILVER:
                return mat_silver;
            case VehicleColour.BLACK:
                return mat_black;
            case VehicleColour.WHITE:
                return mat_white;
            case VehicleColour.ORANGE:
                return mat_orange;
            case VehicleColour.PURPLE:
                return mat_purple;
        }

        return MAT_WHITE;
    }

    public static GameObject GetVehicle(int id) { return registry[id]; }
    public static GameObject GetCar(int id) { return cars[id]; }
    public static GameObject GetVan(int id) { return vans[id]; }
    public static GameObject GetTruck(int id) { return trucks[id]; }
    public static GameObject GetBus(int id) { return busses[id]; }

    public static GameObject GetRandomCar() {
        int id = Random.Range(0, cars.Count);
        return cars[id];
    }

    public static GameObject GetRandomVan() {
        int id = Random.Range(0, vans.Count);
        return vans[id];
    }

    public static GameObject GetRandomTruck() {
        int id = Random.Range(0, trucks.Count);
        return trucks[id];
    }

    public static GameObject GetRandomBus() {
        int id = Random.Range(0, busses.Count);
        return busses[id];
    }
    
    public static int GetTotalVehicles() { return registry.Length; }
    public static int GetTotalCars() { return cars.Count; }
    public static int GetTotalVans() { return vans.Count; }
    public static int GetTotalTrucks() { return trucks.Count; }
    public static int GetTotalBusses() { return busses.Count; }
    
    public static GameObject[] GetVehicleRegistry() { return registry; }
    public static List<GameObject> GetCarRegistry() { return cars; }
    public static List<GameObject> GetVanRegistry() { return vans; }
    public static List<GameObject> GetTruckRegistry() { return trucks; }
    public static List<GameObject> GetBusRegistry() { return busses; }
}

public enum VehicleColour {
    RED,
    BLUE,
    YELLOW,
    GREEN,
    SILVER,
    BLACK,
    WHITE,
    ORANGE,
    PURPLE
}