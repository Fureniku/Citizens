using Tiles.TileManagement;
using UnityEngine;

public class GenerateLargeBuilding : GenerateBuildingBase {

    private int buildingId;
    private EnumDirection rotation;

    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        if (w == 0 && l == 0) {
            rot = rotation;
            return buildingId;
        }

        TileData td = TileRegistry.GetTileFromID(buildingId);

        if (w < td.GetWidth() && l < td.GetLength()) {
            return TileRegistry.REFERENCE.GetId();
        }

        return -1;
    }

    public GenerateLargeBuilding(TilePos startPos, int width, int length, int buildingId, EnumDirection rotation)
        : base(startPos, width, 1, 1, length) {
        this.buildingId = buildingId;
        this.rotation = rotation;
    }
}