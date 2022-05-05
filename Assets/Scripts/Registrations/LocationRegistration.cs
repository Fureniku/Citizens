using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocationRegistration {

    public static Registry allVehicleSpawnersRegistry = new Registry();
    public static Registry allVehicleDestinationsRegistry = new Registry();
    
    public static Registry allPedestrianSpawnersRegistry = new Registry();
    public static Registry allPedestrianDestinationsRegistry = new Registry();
    
    public static Registry RoadSpawnerRegistry = new Registry();
    
    //Vehicle spawner registries
    public static Registry worldEntryVehicle = new Registry();
    public static Registry shopRegistrySpawnerVehicle = new Registry();
    public static Registry houseRegistrySpawnerVehicle = new Registry();

    //Vehicle destination registries
    public static Registry worldExitVehicle = new Registry();
    public static Registry shopRegistryDestVehicle = new Registry();
    public static Registry houseRegistryDestVehicle = new Registry();
    
    //Pedestrian spawner registries
    public static Registry worldEntryPedestrian = new Registry();
    public static Registry shopRegistrySpawnerPedestrian = new Registry();
    public static Registry houseRegistrySpawnerPedestrian = new Registry();
    
    //Pedestrian destination registries
    public static Registry worldExitPedestrian = new Registry();
    public static Registry shopRegistryDestPedestrian = new Registry();
    public static Registry houseRegistryDestPedestrian = new Registry();

    public static TilePos hospitalVehiclePos;
    private static TilePos hospitalPedestrianPos;
    public static TilePos townHallVehiclePos;
    private static TilePos townHallPedestrianPos;
    private static TilePos universityVehiclePos;
    private static TilePos universityPedestrianPos;
    

    public static List<LocationNodeController> nodeControllers = new List<LocationNodeController>();
    public static List<ParkingSpaceNode> carParkSpaces = new List<ParkingSpaceNode>();


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
            if (lnc != null) {
                TileData tileData = lnc.GetParentTile();
                if (!tileData.IsRegistryVersion()) {
    
                    switch (lnc.GetLocationType()) {
                        case LocationType.HOSPITAL:
                            if (lnc.GetSpawnerNodePedestrian() != null || lnc.GetDestinationNodePedestrian() != null) hospitalPedestrianPos = lnc.GetTilePosition();
                            if (lnc.GetDestinationNodeVehicle() != null) hospitalVehiclePos = lnc.GetTilePosition();
                            break;
                        case LocationType.TOWN_HALL:
                            if (lnc.GetSpawnerNodePedestrian() != null || lnc.GetDestinationNodePedestrian() != null) townHallPedestrianPos = lnc.GetTilePosition();
                            if (lnc.GetDestinationNodeVehicle() != null) townHallVehiclePos = lnc.GetTilePosition();
                            break;
                        case LocationType.SHOP:
                            if (lnc.GetSpawnerNodeVehicle() != null) shopRegistrySpawnerVehicle.AddToList(lnc.GetTilePosition());
                            if (lnc.GetDestinationNodeVehicle() != null) shopRegistryDestVehicle.AddToList(lnc.GetTilePosition());
                            if (lnc.GetSpawnerNodePedestrian() != null) shopRegistrySpawnerPedestrian.AddToList(lnc.GetTilePosition());
                            if (lnc.GetDestinationNodePedestrian() != null) shopRegistryDestPedestrian.AddToList(lnc.GetTilePosition());
                            break;
                        case LocationType.HOUSE:
                            if (lnc.GetSpawnerNodeVehicle() != null) houseRegistrySpawnerVehicle.AddToList(lnc.GetTilePosition());
                            if (lnc.GetDestinationNodeVehicle() != null) houseRegistryDestVehicle.AddToList(lnc.GetTilePosition());
                            if (lnc.GetSpawnerNodePedestrian() != null) houseRegistrySpawnerPedestrian.AddToList(lnc.GetTilePosition());
                            if (lnc.GetDestinationNodePedestrian() != null) houseRegistryDestPedestrian.AddToList(lnc.GetTilePosition());
                            break;
                        case LocationType.WORLD_EXIT:
                            if (lnc.GetSpawnerNodeVehicle() != null) worldEntryVehicle.AddToList(lnc.GetTilePosition());
                            if (lnc.GetDestinationNodeVehicle() != null) worldExitVehicle.AddToList(lnc.GetTilePosition());
                            if (lnc.GetSpawnerNodePedestrian() != null) worldEntryPedestrian.AddToList(lnc.GetTilePosition());
                            if (lnc.GetDestinationNodePedestrian() != null) worldExitPedestrian.AddToList(lnc.GetTilePosition());
                            break;
                    }
                }
            } else {
                nodeControllers.RemoveAt(i);
            }
        }
        
        RegisterAllVehicleSpawners();
        RegisterAllVehicleDestinations();
        RegisterAllPedestrianSpawners();
        RegisterAllPedestrianDestinations();
        
        Debug.Log("Location registrations complete. Generated lists with");
        Debug.Log("    - " + allVehicleSpawnersRegistry.GetListSize() + " Vehicle Spawners");
        Debug.Log("    - " + allVehicleDestinationsRegistry.GetListSize() + " Vehicle Destinations");
        Debug.Log("    - " + allPedestrianSpawnersRegistry.GetListSize() + " Pedestrian Spawners");
        Debug.Log("    - " + allPedestrianDestinationsRegistry.GetListSize() + " Pedestrian Destinations");
        Debug.Log("    - " + carParkSpaces.Count + " Car parking space");
    }

    private static void RegisterAllVehicleSpawners() {
        foreach (TilePos pos in shopRegistrySpawnerVehicle.GetList()) { allVehicleSpawnersRegistry.AddToList(pos); }
        foreach (TilePos pos in houseRegistrySpawnerVehicle.GetList()) { allVehicleSpawnersRegistry.AddToList(pos); }
        foreach (TilePos pos in worldEntryVehicle.GetList()) { allVehicleSpawnersRegistry.AddToList(pos); }
    }

    private static void RegisterAllVehicleDestinations() {
        if (hospitalVehiclePos != null) allVehicleDestinationsRegistry.AddToList(hospitalVehiclePos);
        if (townHallVehiclePos != null) allVehicleDestinationsRegistry.AddToList(townHallVehiclePos);
        if (universityVehiclePos != null) allVehicleDestinationsRegistry.AddToList(universityVehiclePos);
        foreach (TilePos pos in shopRegistryDestVehicle.GetList()) { allVehicleDestinationsRegistry.AddToList(pos); }
        foreach (TilePos pos in houseRegistryDestVehicle.GetList()) { allVehicleDestinationsRegistry.AddToList(pos); }
        foreach (TilePos pos in worldExitVehicle.GetList()) { allVehicleDestinationsRegistry.AddToList(pos); }
    }
    
    private static void RegisterAllPedestrianSpawners() {
        if (hospitalPedestrianPos != null) allPedestrianSpawnersRegistry.AddToList(hospitalPedestrianPos);
        if (townHallPedestrianPos != null) allPedestrianSpawnersRegistry.AddToList(townHallPedestrianPos);
        if (universityPedestrianPos != null) allPedestrianSpawnersRegistry.AddToList(universityPedestrianPos);
        foreach (TilePos pos in shopRegistrySpawnerPedestrian.GetList()) { allPedestrianSpawnersRegistry.AddToList(pos); }
        foreach (TilePos pos in houseRegistrySpawnerPedestrian.GetList()) { allPedestrianSpawnersRegistry.AddToList(pos); }
        foreach (TilePos pos in worldEntryPedestrian.GetList()) { allPedestrianSpawnersRegistry.AddToList(pos); }
    }

    private static void RegisterAllPedestrianDestinations() {
        if (hospitalPedestrianPos != null) allPedestrianDestinationsRegistry.AddToList(hospitalPedestrianPos);
        if (townHallPedestrianPos != null) allPedestrianDestinationsRegistry.AddToList(townHallPedestrianPos);
        if (universityPedestrianPos != null) allPedestrianDestinationsRegistry.AddToList(universityPedestrianPos);
        foreach (TilePos pos in shopRegistryDestPedestrian.GetList()) { allPedestrianDestinationsRegistry.AddToList(pos); }
        foreach (TilePos pos in houseRegistryDestPedestrian.GetList()) { allPedestrianDestinationsRegistry.AddToList(pos); }
        foreach (TilePos pos in worldExitPedestrian.GetList()) { allPedestrianDestinationsRegistry.AddToList(pos); }
    }

    public static void AddToList(LocationNodeController lnc) {
        nodeControllers.Add(lnc);
    }

    public static ParkingSpaceNode GetRandomParkingSpaceNode() {
        return carParkSpaces[Random.Range(0, carParkSpaces.Count)];
    }
}

public enum LocationType {
    HOSPITAL,
    TOWN_HALL,
    UNIVERSITY,
    SHOP,
    HOUSE,
    WORLD_EXIT
}