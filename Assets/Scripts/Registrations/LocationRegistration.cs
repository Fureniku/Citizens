using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocationRegistration {

    public static Registry allSpawnersRegistry = new Registry();
    public static Registry allDestinationsRegistry = new Registry();
    public static Registry RoadSpawnerRegistry = new Registry();
    
    //Vehicle spawner registries
    public static Registry worldEntryVehicle = new Registry();

    //Vehicle destination registries
    public static Registry hospitalRegistry = new Registry();
    public static Registry worldExitVehicle = new Registry();
    
    //Pedestrian registries
    public static Registry shopRegistryPedestrian = new Registry();
    public static Registry houseRegistryPedestrian = new Registry();
    public static Registry worldExitPedestrian = new Registry();
    

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

                if (lnc.GetSpawnerNode() != null) allSpawnersRegistry.AddToList(lnc.GetTilePosition()); 
                if (lnc.GetDestinationNode() != null) allDestinationsRegistry.AddToList(lnc.GetTilePosition());
                

                switch (lnc.GetLocationType()) {
                    case LocationType.HOSPITAL:
                        if (lnc.GetSpawnerNode() != null) {}
                        if (lnc.GetDestinationNode() != null) hospitalRegistry.AddToList(lnc.GetTilePosition());
                        break;
                    case LocationType.SHOP_PEDESTRIAN:
                        if (lnc.GetSpawnerNode() != null) {}
                        if (lnc.GetDestinationNode() != null) shopRegistryPedestrian.AddToList(lnc.GetTilePosition());
                        break;
                    case LocationType.HOUSE_PEDESTRIAN:
                        if (lnc.GetSpawnerNode() != null) {}
                        if (lnc.GetDestinationNode() != null) houseRegistryPedestrian.AddToList(lnc.GetTilePosition());
                        break;
                    case LocationType.WORLD_EXIT_VEHICLE:
                        if (lnc.GetSpawnerNode() != null) worldEntryVehicle.AddToList(lnc.GetTilePosition());
                        if (lnc.GetDestinationNode() != null) worldExitVehicle.AddToList(lnc.GetTilePosition());
                        break;
                    case LocationType.WORLD_EXIT_PEDESTRIAN:
                        if (lnc.GetSpawnerNode() != null) {}
                        if (lnc.GetDestinationNode() != null) worldExitPedestrian.AddToList(lnc.GetTilePosition());
                        break;
                }
            }
        }
    }

    public static void AddToList(LocationNodeController lnc) {
        nodeControllers.Add(lnc);
    }
}

public enum LocationType {
    HOSPITAL,
    SHOP_PEDESTRIAN,
    HOUSE_PEDESTRIAN,
    WORLD_EXIT_VEHICLE,
    WORLD_EXIT_PEDESTRIAN,
}