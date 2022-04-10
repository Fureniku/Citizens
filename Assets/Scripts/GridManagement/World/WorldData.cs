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

    //Objects
    [SerializeField] private GameObject navMeshRoad = null; //Road navmesh
    [SerializeField] private GameObject navMeshSidewalk = null; //Sidewalk navmesh

    [ReadOnly, SerializeField] private bool existingWorld = false; //Whether a world can be loaded

    [SerializeField] private int chunkGenPercent = 0; //Percentage of chunk generation completed.

    [SerializeField] private int worldRoadSpawnTiles;
    [SerializeField] private int worldRoadDestTiles;

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
    public void SetChunkGenPercent(int percent) => chunkGenPercent = percent;
    public void SetNavMeshRoad(GameObject nm) => navMeshRoad = nm;
    public void SetNavMeshSidewalk(GameObject nm) => navMeshSidewalk = nm;
    
    public GameObject GetNavMeshRoad() { return navMeshRoad; }
    public GameObject GetNavMeshSidewalk() { return navMeshSidewalk; }
    public string GetWorldName() { return worldName; }
    public int GetWorldSize() { return worldSize; }
    public bool SavingEnabled() { return saving; }
    public bool DoesWorldExist() { return existingWorld; }
    public void SetWorldExists() => existingWorld = true;

    void Update() {
        worldRoadSpawnTiles = DestinationRegistration.RoadSpawnerRegistry.GetListSize();
        worldRoadDestTiles = DestinationRegistration.RoadDestinationRegistry.GetListSize();
    }
}