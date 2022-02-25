using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileRoad : TileData {

    [SerializeField] private RoadType roadType = RoadType.ROAD_STRAIGHT;
    
    void Start() {
        switch (roadType) {
            case RoadType.ROAD_STRAIGHT:
                tileId = TileRegistry.STRAIGHT_ROAD_1x1.GetId();
                name = TileRegistry.STRAIGHT_ROAD_1x1.GetName();
                break;
            case RoadType.ROAD_CORNER:
                tileId = TileRegistry.CORNER_ROAD_1x1.GetId();
                name = TileRegistry.CORNER_ROAD_1x1.GetName();
                break;
            case RoadType.ROAD_T_JUNCT:
                tileId = TileRegistry.T_JUNCT_ROAD_1x1.GetId();
                name = TileRegistry.T_JUNCT_ROAD_1x1.GetName();
                break;
            case RoadType.ROAD_CROSSROAD:
                tileId = TileRegistry.CROSSROAD_ROAD_1x1.GetId();
                name = TileRegistry.CROSSROAD_ROAD_1x1.GetName();
                break;
            case RoadType.ROAD_CROSSROAD_CONTROLLED:
                tileId = TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetId();
                name = TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetName();
                break;
        }
        width = 1;
        length = 1;
    }
    
    public override JProperty SerializeTile(int row, int col) {
        TileData data = GridManager.Instance.GetGridTile(row, col);
            
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetGridPos().x));
        jObj.Add(new JProperty("col", data.GetGridPos().z));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }

    public override void DeserializeTile(JObject json) {
        SetId(ParseInt(json.GetValue("id")));
        SetName(tileName);
        SetRotation(Direction.GetDirection(ParseInt(json.GetValue("rotation"))));
        SetRowCol(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col")));
    }
}

public enum RoadType {
    ROAD_STRAIGHT,
    ROAD_CORNER,
    ROAD_T_JUNCT,
    ROAD_CROSSROAD,
    ROAD_CROSSROAD_CONTROLLED
}
