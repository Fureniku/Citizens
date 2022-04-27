using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    [SerializeField] private Door[] doors;
    [SerializeField] private bool doorsOpen;

    private void FixedUpdate() {
        if (doorsOpen) {
            OpenDoors();
        } else {
            CloseDoors();
        }
    }

    private void OpenDoors() {
        for (int i = 0; i < doors.Length; i++) {
            doors[i].OpenDoor();
        }
    }
    
    private void CloseDoors() {
        for (int i = 0; i < doors.Length; i++) {
            doors[i].CloseDoor();
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("trigger entered");
        doorsOpen = true;
    }

    private void OnTriggerExit(Collider other) {
        doorsOpen = false;
    }
}
