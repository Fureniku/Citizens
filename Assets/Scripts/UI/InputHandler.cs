using Tiles.TileManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

public class InputHandler : MonoBehaviour {
    
    private Camera cam;
    private GameObject selectedGo;

    private bool isPaused;
    private bool toggleChat;
    
    [SerializeField] private GameObject scenarioCanvas;
    [SerializeField] private GameObject pauseScreenCanvas;
    [SerializeField] private GameObject mainCanvas;

    void Start() {
        cam = GetComponent<Camera>();
    } 

    void Update() {
        if (World.Instance.IsWorldFullyLoaded()) {
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

            if (Input.GetKeyDown(KeyCode.Escape)) {
                TogglePause();
            }

            if (Input.GetKeyDown(KeyCode.Return)) {
                ToggleChat();
            }
            
            if (Input.GetKeyDown(KeyCode.F1)) {
                mainCanvas.SetActive(!mainCanvas.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.F5)) {
                if (Cursor.lockState != CursorLockMode.Locked) {
                    Cursor.lockState = CursorLockMode.Locked;
                } else {
                    Cursor.lockState = CursorLockMode.Confined;
                }
                
            }
        
            if (Input.GetKeyDown(KeyCode.F2)) {
                Time.timeScale = 0.25f;
                World.Instance.SendChatMessage("World", "Setting world speed to 0.25x");
            }
        
            if (Input.GetKeyDown(KeyCode.F3)) {
                Time.timeScale = 1.0f;
                World.Instance.SendChatMessage("World", "Setting world speed to 1.0x");
            }
        
            if (Input.GetKeyDown(KeyCode.F4)) {
                Time.timeScale = 2.0f;
                World.Instance.SendChatMessage("World", "Setting world speed to 2.0x");
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                if (selectedGo.GetComponent<BaseAgent>() != null) {
                    Vector3 target = selectedGo.GetComponent<BaseAgent>().GetFinalKnownDestination().transform.position;
                    cam.transform.position = new Vector3(target.x, 10.0f, target.z);
                }
            }
        }
    }

    public void TogglePause() {
        if (!isPaused) {
            ClearSelection();
            World.Instance.GetChatController().EnableFade();
            pauseScreenCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0.0f;
        } else {
            pauseScreenCanvas.SetActive(false);
            mainCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1.0f;
        }

        isPaused = !isPaused;
    }

    private void ToggleChat() {
        if (!toggleChat) {
            World.Instance.GetChatController().ShowChatWindow();
            World.Instance.GetChatController().DisableFade();
            Cursor.lockState = CursorLockMode.Confined;
        } else {
            World.Instance.GetChatController().EnableFade();
            Cursor.lockState = CursorLockMode.Locked;
        }

        toggleChat = !toggleChat;
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

    public void ReturnToMenu() {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
