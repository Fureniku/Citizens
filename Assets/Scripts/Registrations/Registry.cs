using System.Collections;
using UnityEngine;

public class Registry {
    
    private ArrayList arrayList = new ArrayList();
    private TileType type;

    public Registry(TileType type) {
        this.type = type;
    }

    public int GetListSize() {
        return arrayList.Count;
    }

    public void AddToList(TilePos pos) {
        if (!arrayList.Contains(pos)) {
            arrayList.Add(pos);
        }
    }

    public TilePos GetFromList(int id) {
        return (TilePos) arrayList[id];
    }

    public TilePos GetAtRandom() {
        int rnd = Random.Range(0, arrayList.Count);
        Debug.Log("Going to location " + rnd + " which is " + (TilePos) arrayList[rnd]);
        return (TilePos) arrayList[rnd];
    }

    public void BuildList() {
        Debug.Log("Building list...");
        for (int i = 0; i < World.Instance.GetGridManager().GetSize() * Chunk.size; i++) {
            for (int j = 0; j < World.Instance.GetGridManager().GetSize() * Chunk.size; j++) {
                TilePos tilePos = new TilePos(i, j);
                TileData tileData = World.Instance.GetGridManager().GetTile(tilePos);
                if (tileData is TileRoad) {
                    AddToList(tilePos);
                }
            }
        }
        Debug.Log("List built with " + GetListSize() + " entries.");
    }
}