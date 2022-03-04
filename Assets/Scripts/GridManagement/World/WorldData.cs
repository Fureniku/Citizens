using UnityEngine;

public class WorldData : MonoBehaviour {

    [SerializeField] private string worldName = "";
    [SerializeField] private bool saving = false;

    [SerializeField] private EnumWorldState state;
    
    [SerializeField] private int worldSize = 0;
    [SerializeField] private int chunkGenPercent = 0;

    [SerializeField] private int worldRoadTiles;

    public void SetWorldName(string wName) => worldName = wName;
    public void SetWorldSaving(bool save) => saving = save;
    public void SetWorldState(EnumWorldState s) => state = s;
    
    public void SetWorldSize(int size) => worldSize = size;
    public void SetChunkGenPercent(int percent) => chunkGenPercent = percent;

    void Update() {
        worldRoadTiles = DestinationRegistration.RoadRegistry.GetListSize();
    }

}