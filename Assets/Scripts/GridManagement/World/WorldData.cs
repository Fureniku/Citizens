using UnityEngine;

public class WorldData : MonoBehaviour {

    private static WorldData _instance;

    public static WorldData Instance {
        get { return _instance; }
    }

    //Settings
    [SerializeField] private string worldName = "Default World"; //Name of the world
    [SerializeField] private bool saving = false; //Whether saving is enabled
    [SerializeField] private int worldSize = 3; //World's size in chunks

    [SerializeField] private bool existingWorld = false; //Whether a world can be loaded

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetWorldName(string wName) => worldName = wName;
    public void SetWorldSaving(bool save) => saving = save;
    
    public void SetWorldSize(int size) => worldSize = size;

    public string GetWorldName() { return worldName; }
    public int GetWorldSize() { return worldSize; }
    public bool SavingEnabled() { return saving; }
    public bool DoesWorldExist() { return existingWorld; }
    public void SetWorldExists() => existingWorld = true;
}