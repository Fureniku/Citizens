using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProperties : MonoBehaviour {

    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject selectionInfo;
    [SerializeField] private GameObject selectionInfoBuilding;
    [SerializeField] private GameObject selectionInfoVehicle;
    [SerializeField] private GameObject selectionInfoPedestrian;

    private InputHandler inputHandler;
    private CameraController cameraController;
    private World world;

    private bool camFollow = false;
    private bool fDown = false;

    void Start() {
        inputHandler = cam.GetComponent<InputHandler>();
        cameraController = cam.GetComponent<CameraController>();
        world = World.Instance;
    }

    private void FixedUpdate() {
        if (inputHandler.SelectedObject() != null) {
            if (inputHandler.SelectedObject().GetComponent<TileData>() != null) {
                TileData td = inputHandler.SelectedObject().GetComponent<TileData>();
                SetCamFollow(false);
                if (td is TileBuilding) {
                    SetPanelEnabled(SelectionType.BUILDING);
                    selectionInfoBuilding.GetComponent<SelectionInfo>().SetSelectionInfo(td);
                } else {
                    SetPanelEnabled(SelectionType.STANDARD);
                    selectionInfo.GetComponent<SelectionInfo>().SetSelectionInfo(td);
                }
                
            } else if (inputHandler.SelectedObject().GetComponent<VehicleAgent>() != null) {
                SetPanelEnabled(SelectionType.VEHICLE);
                selectionInfoVehicle.GetComponent<VehicleSelectionInfo>().SetSelectionInfo(inputHandler.SelectedObject().GetComponent<VehicleAgent>());
                if (Input.GetKeyDown(KeyCode.F) && !fDown) {
                    fDown = true;
                    if (!camFollow) {
                        SetCamFollow(true, inputHandler.SelectedObject());
                    }
                    else {
                        SetCamFollow(false);
                    }
                }
            } else if (inputHandler.SelectedObject().GetComponent<PedestrianAgent>() != null) {
                SetPanelEnabled(SelectionType.PEDESTRIAN);
                selectionInfoPedestrian.GetComponent<PedestrianSelectionInfo>().SetSelectionInfo(inputHandler.SelectedObject().GetComponent<PedestrianAgent>());
                if (Input.GetKeyDown(KeyCode.F) && !fDown) {
                    fDown = true;
                    if (!camFollow) {
                        SetCamFollow(true, inputHandler.SelectedObject());
                    }
                    else {
                        SetCamFollow(false);
                    }
                }
            }
        }
        else {
            SetCamFollow(false);
            SetPanelEnabled(SelectionType.NONE);
            if (Input.GetKey(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (Input.GetKeyUp(KeyCode.F)) {
            fDown = false;
        }
    }
    
    private void SetCamFollow(bool follow, GameObject obj = null) {
        camFollow = follow;
        if (follow && obj != null) {
            GameObject camPos = obj.GetComponent<BaseAgent>().GetCamPos();
            cam.transform.parent = camPos.transform;
            cam.transform.rotation = camPos.transform.rotation;
            cam.transform.position = camPos.transform.position;
        } else {
            cam.transform.parent = transform;
        }
    }

    private void SetPanelEnabled(SelectionType type) {
        selectionInfo.SetActive(false);
        selectionInfoBuilding.SetActive(false);
        selectionInfoVehicle.SetActive(false);
        selectionInfoPedestrian.SetActive(false);
        switch (type) {
            case SelectionType.STANDARD:
                selectionInfo.SetActive(true);
                break;
            case SelectionType.BUILDING:
                selectionInfoBuilding.SetActive(true);
                break;
            case SelectionType.VEHICLE:
                selectionInfoVehicle.SetActive(true);
                break;
            case SelectionType.PEDESTRIAN:
                selectionInfoPedestrian.SetActive(true);
                break;
        }
    }
    
    public void Resume() {
        Cursor.lockState = CursorLockMode.Locked;
    }
}

public enum SelectionType {
    STANDARD,
    BUILDING,
    VEHICLE,
    PEDESTRIAN,
    NONE
}