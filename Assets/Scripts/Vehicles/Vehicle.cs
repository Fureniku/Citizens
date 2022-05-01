using UnityEngine;

public class Vehicle : MonoBehaviour {

    [SerializeField] private Seat[] seats;
    [SerializeField] private string vehicleName;
    [SerializeField] private VehicleType vehicleType;
    [SerializeField] private VehicleColour vehicleColour;
    [SerializeField] private VehicleRegistry vehicleRegistry;
    [SerializeField] private GameObject[] colouredParts;

    public int GetMaxSeats() {
        return seats.Length;
    }
    
    public VehicleType GetVehicleType() {
        return vehicleType;
    }

    public string GetName() {
        return vehicleName;
    }

    public VehicleColour GetColour() {
        return vehicleColour;
    }
    
    private void OnValidate() {
        if (colouredParts.Length > 0) {
            for (int i = 0; i < colouredParts.Length; i++) {
                colouredParts[i].GetComponent<MeshRenderer>().material = vehicleRegistry.GetMaterialNonStatic(vehicleColour);
            }
        }
    }

    public Seat GetNextAvailableSeat() {
        for (int i = 0; i < seats.Length; i++) {
            if (seats[i].IsAvailable()) {
                return seats[i];
            }
        }

        return null;
    }

    public Seat GetSeat(int id) {
        return seats[id];
    }

    public bool IsVehicleFull() {
        for (int i = 0; i < seats.Length; i++) {
            if (seats[i].IsAvailable()) {
                return false;
            }
        }
        return true;
    }
    
    //agent enter vehicle
    //agent exit vehicle
}

public enum VehicleType {
    CAR,
    VAN,
    TRUCK,
    BUS
}