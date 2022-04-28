using cakeslice;
using Tiles.TileManagement;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    
    private Camera cam;
    private GameObject selectedGo;
    
    [SerializeField] private GameObject scenarioCanvas;

    // Start is called before the first frame update
    void Start() {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked) {
            int layerMask = LayerMask.GetMask("Clickable");
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000, layerMask)) {
                Debug.Log("Clicked " + hit.transform.gameObject.name);
                ToggleHighlight(false); //Turn off previous selection

                selectedGo = hit.transform.gameObject;
                ToggleHighlight(true);

                if (hit.transform.gameObject.CompareTag("ScenarioSpawner")) {
                    Scenarios.Scenarios.Instance.SetSpawnerObject(hit.transform.gameObject);
                    scenarioCanvas.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                } else if (hit.transform.gameObject.CompareTag("Pedestrian")) {
                    
                }
            }
        }

        if (Input.GetKey(KeyCode.Escape)) {
            ClearSelection();
        }

        if (Input.GetKeyDown(KeyCode.F1)) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ToggleHighlight(bool highlight) {
        if (selectedGo != null) {
            if (selectedGo.GetComponent<Outline>() != null) {
                selectedGo.GetComponent<Outline>().enabled = highlight;
            }
            for (int i = 0; i < selectedGo.transform.childCount; i++) {
                GameObject child = selectedGo.transform.GetChild(i).gameObject;
                if (child.GetComponent<Outline>() != null) {
                    child.GetComponent<Outline>().enabled = highlight;
                }
            }
        }
    }

    public void ClearSelection() {
        ToggleHighlight(false);
        selectedGo = null;
    }

    public GameObject SelectedObject() {
        return selectedGo;
    }
}
