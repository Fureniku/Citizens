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
            case RoadType.T_JUNCT_ROAD_1x1_SINGLE_IN:
                tileId = TileRegistry.T_JUNCT_ROAD_1x1_SINGLE_IN.GetId();
                name = TileRegistry.T_JUNCT_ROAD_1x1_SINGLE_IN.GetName();
                break;
            case RoadType.T_JUNCT_ROAD_1x1_SINGLE_OUT:
                tileId = TileRegistry.T_JUNCT_ROAD_1x1_SINGLE_OUT.GetId();
                name = TileRegistry.T_JUNCT_ROAD_1x1_SINGLE_OUT.GetName();
                break;
            case RoadType.ZEBRA_CROSSING:
                tileId = TileRegistry.ZEBRA_CROSSING_1x1.GetId();
                name = TileRegistry.ZEBRA_CROSSING_1x1.GetName();
                break;
            case RoadType.PELICAN_CROSSING:
                tileId = TileRegistry.PELICAN_CROSSING_1x1.GetId();
                name = TileRegistry.PELICAN_CROSSING_1x1.GetName();
                break;
            case RoadType.CROSSING_APPROACH:
                tileId = TileRegistry.CROSSING_APPROACH_1x1.GetId();
                name = TileRegistry.CROSSING_APPROACH_1x1.GetName();
                break;
        }
        width = 1;
        length = 1;
    }
    
    public override JProperty SerializeTile(TileData data, int row, int col) {
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
        SetLocalPos(new LocalPos(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col"))));
        
        SetInitialPos();
    }
    
    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<MeshRenderer>() != null) {
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public override void Create() {
        CreateBase();
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<MeshRenderer>() != null) {
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}

public enum RoadType {
    ROAD_STRAIGHT,
    ROAD_CORNER,
    ROAD_T_JUNCT,
    ROAD_CROSSROAD,
    ROAD_CROSSROAD_CONTROLLED,
    T_JUNCT_ROAD_1x1_SINGLE_IN,
    T_JUNCT_ROAD_1x1_SINGLE_OUT,
    ZEBRA_CROSSING,
    PELICAN_CROSSING,
    CROSSING_APPROACH
}
