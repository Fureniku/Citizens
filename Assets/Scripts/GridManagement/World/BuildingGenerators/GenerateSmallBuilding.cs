using System;
using Tiles.TileManagement;

public class GenerateSmallBuilding : GenerateBuildingBase {

    private BuildingTypeRegister btr;

    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        return PlaceSimpleObject(w, l, ref rot, btr);
    }
    
    public override void PostGenerate() {}

    public GenerateSmallBuilding(TilePos startPos, int width, int length, BuildingTypeRegister btr)
        : base(startPos, width, 1, 1, length) {
        this.btr = btr;
    }
}