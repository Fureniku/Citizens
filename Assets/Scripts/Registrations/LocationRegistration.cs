using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DestinationRegistration {

    public static Registry allDestinationsRegistry = new Registry();
    public static Registry RoadSpawnerRegistry = new Registry();

    public static Registry hospitalRegistry = new Registry();
    public static Registry worldExit = new Registry();

    public static List<LocationNodeController> nodeControllers = new List<LocationNodeController>();


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
                    }
                }
            }
        }
        
        Debug.Log("Spawner list built with " + RoadSpawnerRegistry.GetListSize() + " entries.");

        for (int i = 0; i < nodeControllers.Count; i++) {
            LocationNodeController lnc = nodeControllers[i];
            TileData tileData = lnc.GetParentTile();
            if (!tileData.IsRegistryVersion()) {
                Debug.Log("Adding " + tileData.GetName() + " at " + tileData.GetTilePos() + " with type " + lnc.GetLocationType() + " to destination registry");
                if (lnc.GetDestinationNode() != null) {
                    allDestinationsRegistry.AddToList(lnc.GetTilePosition());
                }

                switch (lnc.GetLocationType()) {
                    case LocationType.HOSPITAL:
                        hospitalRegistry.AddToList(lnc.GetTilePosition());
                        break;
                    case LocationType.WORLD_EXIT:
                        worldExit.AddToList(lnc.GetTilePosition());
                        break;
                }
            }
        }
        
        Debug.Log("Destination lists populated with " + hospitalRegistry.GetListSize() + " hospitals, " + worldExit.GetListSize() + " world exits.");
    }

    public static void AddToList(LocationNodeController lnc) {
        nodeControllers.Add(lnc);
    }
}

public enum LocationType {
    HOSPITAL,
    WORLD_EXIT
}