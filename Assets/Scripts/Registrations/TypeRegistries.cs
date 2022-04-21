using System.Collections.Generic;

public class TypeRegistries {
    
    public static List<Tile> houses = new List<Tile>();
    public static List<Tile> shops = new List<Tile>();
    
    public static void PopulateRegistries() {
        foreach (Tile tile in TileRegistry.GetRegistry()) {
            if (tile.GetTileType() == TileType.BUILDING_SHOP) {
                shops.Add(tile);
            } else if (tile.GetTileType() == TileType.BUILDING_HOUSE) {
                houses.Add(tile);
            }
        }
    }
    
    public static int GetTotalHouses() { return houses.Count; }
    public static int GetTotalShops() { return shops.Count; }
    
    public static Tile GetHouse(int id) { return houses[id]; }
    public static Tile GetShop(int id) { return shops[id]; }
}