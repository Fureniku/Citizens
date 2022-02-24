using Tiles.TileManagement;

public class TileGrass : TileData {
    
    void Start() {
        tileName = "Grass";
        tileId = TileRegistry.GRASS.GetId();
        width = 1;
        length = 1;
        halfRotations = false;
        rotation = EnumTileDirection.SOUTH;
    }
}
