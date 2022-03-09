using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProperties : MonoBehaviour {

    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject editCanvas;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject selectionInfo;
    [SerializeField] private GameObject selectionInfoBuilding;

    private InputHandler inputHandler;
    private CameraController cameraController;
    private World world;

    private bool mouseMenu;

    void Start() {
        inputHandler = camera.GetComponent<InputHandler>();
        cameraController = camera.GetComponent<CameraController>();
        world = World.Instance;
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.E)) {
            Cursor.lockState = CursorLockMode.None;
            editCanvas.SetActive(true);
            mouseMenu = true;
        }

        if (inputHandler.SelectedObject() != null) {
            TileData td = inputHandler.SelectedObject().GetComponent<TileData>();
            if (td is TileBuilding) {
                selectionInfoBuilding.SetActive(true);
                selectionInfoBuilding.GetComponent<SelectionInfo>().SetSelectionInfo(td);
            } else {
                selectionInfo.SetActive(true);
                selectionInfo.GetComponent<SelectionInfo>().SetSelectionInfo(td);
            }
            
        }
        else {
            selectionInfo.SetActive(false);
            selectionInfoBuilding.SetActive(false);
            if (Input.GetKey(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.None;
            }
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
