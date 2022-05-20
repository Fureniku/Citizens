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
    [SerializeField] private int worldSeed = 20; //World's seed
    
    [SerializeField] private int initialVehicles = 20; //World's size in chunks
    [SerializeField] private int maxVehicles = 30; //World's size in chunks
    [SerializeField] private int initialPedestrians = 10; //World's size in chunks
    [SerializeField] private int maxPedestrians = 25; //World's size in chunks

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
    public void SetWorldSeed(int seed) => worldSeed = seed;
    public void SetInitVehicles(int i) => initialVehicles = i;
    public void SetMaxVehicles(int i) => maxVehicles = i;
    public void SetInitPeds(int i) => initialPedestrians = i;
    public void SetMaxPeds(int i) => maxPedestrians = i;

    public string GetWorldName() { return worldName; }
    public int GetWorldSize() { return worldSize; }
    public int GetWorldSeed() { return worldSeed; }
    public int GetInitVehicles() { return initialVehicles; }
    public int GetMaxVehicles() { return maxVehicles; }
    public int GetInitPeds() { return initialPedestrians; }
    public int GetMaxPeds() { return maxPedestrians; }
    
    
    public bool SavingEnabled() { return saving; }
    public bool DoesWorldExist() { return existingWorld; }
    public void SetWorldExists() => existingWorld = true;
}