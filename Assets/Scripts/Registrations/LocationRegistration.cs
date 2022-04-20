using System.Collections;
using UnityEngine;

public static class DestinationRegistration {

    public static Registry RoadDestinationRegistry = new Registry(TileType.ROAD);
    public static Registry RoadSpawnerRegistry = new Registry(TileType.AIR);


    public static void BuildLists() {
        Debug.Log("Building list...");
        int size = World.Instance.GetChunkManager().GetSize() * Chunk.size;
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                TilePos tilePos = new TilePos(i, j);
                TileData tileData = World.Instance.GetChunkManager().GetTile(tilePos);
                if (tileData != null) {
                    if (tileData.GetTile() == TileRegistry.STRAIGHT_ROAD_1x1) {
                        RoadSpawnerRegistry.AddToList(tilePos);
                    } else if (tileData.GetTile() == TileRegistry.ROAD_WORLD_EXIT) {
                        RoadDestinationRegistry.AddToList(tilePos);
                    }
                }
            }
        }
        
        Debug.Log("Spawner list built with " + RoadSpawnerRegistry.GetListSize() + " entries.");
        Debug.Log("Destination list built with " + RoadDestinationRegistry.GetListSize() + " entries.");
    }
}