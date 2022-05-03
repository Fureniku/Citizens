using Tiles.TileManagement;
using UnityEngine;

public class GenerateTownHall : GenerateLargeBuildingBase {
    
    public override void PostGenerate() {
        int startW = 0;
        int startL = 0;

        if (width  >= building.GetWidth()  + 4) startW = width  / 2 - building.GetWidth()  / 2;
        if (length >= building.GetLength() + 4) startL = length / 2 - building.GetLength() / 2;
        
        TilePos placePos = new TilePos(startPos.x + startW, startPos.z + startL);

        TilePos junctCP = new TilePos(placePos.x - 1, placePos.z + 1);
        TilePos crossing1 = new TilePos(placePos.x - 1, placePos.z + 5);
        TilePos crossing2 = new TilePos(placePos.x + 2, placePos.z + 8);

        SetJunctionIn(junctCP, EnumDirection.NORTH);
        SetCrossing(crossing1, EnumDirection.NORTH);
        SetCrossing(crossing2, EnumDirection.EAST);
        
        TilePos scenarioTilePos = new TilePos(placePos.x + 4, placePos.z + 6);
        Vector3 scenarioPos = new Vector3(scenarioTilePos.GetWorldPos().x, 11.5f, scenarioTilePos.GetWorldPos().z-1);

        Debug.Log("Setting scenario pos to " +scenarioPos);
        Scenarios.Scenarios.Instance.GetStartPosition().transform.position = scenarioPos;
    }
    

    public GenerateTownHall(Section section, int buildingId, EnumDirection rotation, TileData building)
        : base(section, buildingId, rotation, building) {
    }
}