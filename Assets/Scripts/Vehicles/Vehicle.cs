using UnityEngine;

public class Vehicle : MonoBehaviour {

    [SerializeField] private Seat[] seats;

    public int GetMaxSeats() {
        return seats.Length;
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