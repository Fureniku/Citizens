using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProperties : MonoBehaviour {

    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject editCanvas;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject selectionInfo;
    [SerializeField] private GameObject selectionInfoBuilding;
    [SerializeField] private GameObject selectionInfoVehicle;

    private InputHandler inputHandler;
    private CameraController cameraController;
    private World world;

    private bool mouseMenu;
    private bool camFollow = false;
    private bool fDown = false;

    void Start() {
        inputHandler = camera.GetComponent<InputHandler>();
        cameraController = camera.GetComponent<CameraController>();
        world = World.Instance;
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.E)) {
            Cursor.lockState = CursorLockMode.Confined;
            editCanvas.SetActive(true);
            mouseMenu = true;
        }

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
                
            } else if (inputHandler.SelectedObject().GetComponent<UnityEngine.AI.NavMeshAgent>() != null) {
                SetPanelEnabled(SelectionType.VEHICLE);
                selectionInfoVehicle.GetComponent<VehicleSelectionInfo>().SetSelectionInfo(inputHandler.SelectedObject().GetComponent<TestAgent>());
                if (Input.GetKeyDown(KeyCode.F) && !fDown) {
                    Debug.Log("Key down :)");
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
            camera.transform.rotation = Quaternion.identity;
            camera.transform.parent = obj.transform;
            Vector3 position = obj.transform.position;
            camera.transform.position = new Vector3(position.x, position.y+15, position.z-35);
        } else {
            camera.transform.parent = transform;
        }
    }

    private void SetPanelEnabled(SelectionType type) {
        selectionInfo.SetActive(false);
        selectionInfoBuilding.SetActive(false);
        selectionInfoVehicle.SetActive(false);
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
        }
    }
    
    public void Resume() {
        Cursor.lockState = CursorLockMode.Locked;
        editCanvas.SetActive(false);
        mouseMenu = false;
    }
    
    public void EditMode() {
        inputHandler.EditMode();
    }
}

public enum SelectionType {
    STANDARD,
    BUILDING,
    VEHICLE,
    PEDESTRIAN,
    NONE
}