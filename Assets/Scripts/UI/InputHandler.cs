using cakeslice;
using Tiles.TileManagement;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    
    private Camera cam;
    private GameObject selectedGo;
    
    [SerializeField] private GameObject tileList;

    // Start is called before the first frame update
    void Start() {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            int layerMask = LayerMask.GetMask("Clickable");
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000, layerMask)) {
                Debug.Log("Clicked " + hit.transform.gameObject.name);
                ToggleHighlight(false); //Turn off previous selection

                selectedGo = hit.transform.gameObject;
                ToggleHighlight(true);
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
        tileList.SetActive(false);
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

    public void EditMode() {
        tileList.SetActive(!tileList.activeSelf);
        tileList.transform.position = Input.mousePosition;
    }

    public void ReplaceTile(int id) {
        if (selectedGo != null) {
            World.Instance.GetGridManager().SetTile(TilePos.GetGridPosFromLocation(selectedGo.transform.position), id, EnumDirection.NORTH);
        }
    }

    public GameObject SelectedObject() {
        return selectedGo;
    }
}
