using System.Collections.Generic;
using UnityEngine;

public class TypeRegistries {

    public static BuildingTypeRegister SHOPS = new BuildingTypeRegister();
    public static BuildingTypeRegister HOUSES = new BuildingTypeRegister();
    
    public static void PopulateRegistries() {
        foreach (Tile tile in TileRegistry.GetRegistry()) {
            if (tile.GetTileType() == TileType.BUILDING_SHOP) {
                SHOPS.AddEntry(tile);
            } else if (tile.GetTileType() == TileType.BUILDING_HOUSE) {
                HOUSES.AddEntry(tile);
            }
        }
    }
}