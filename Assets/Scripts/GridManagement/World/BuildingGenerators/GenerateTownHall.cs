using Tiles.TileManagement;
using UnityEngine;

public class GenerateTownHall : GenerateLargeBuildingBase {
    
    public override void PostGenerate() {
        int startW = 0;
        int startL = 0;

        if (width  >= building.GetWidth()  + 4) startW = width  / 2 - building.GetWidth()  / 2;
        if (length >= building.GetLength() + 4) startL = length / 2 - building.GetLength() / 2;
        
        TilePos placePos = new TilePos(startPos.x + startW, startPos.z + startL);

        TilePos junctAE = new TilePos(placePos.x, placePos.z - 1);
        TilePos carPark1 = new TilePos(placePos.x - 1, placePos.z + 5);
        TilePos carPark2 = new TilePos(placePos.x + 2, placePos.z + 8);

        SetCrossing(junctAE, EnumDirection.WEST);
        SetCrossing(carPark1, EnumDirection.NORTH);
        SetCrossing(carPark2, EnumDirection.EAST);
        
        TilePos scenarioTilePos = new TilePos(placePos.x + 4, placePos.z + 6);
        Vector3 scenarioPos = new Vector3(scenarioTilePos.GetWorldPos().x, 7.5f, scenarioTilePos.GetWorldPos().z);

        Debug.Log("Setting scenario pos to " +scenarioPos);
        Scenarios.Scenarios.Instance.GetStartPosition().transform.position = scenarioPos;
    }
    

    public GenerateTownHall(Section section, int buildingId, EnumDirection rotation, TileData building)
        : base(section, buildingId, rotation, building) {
    }
}