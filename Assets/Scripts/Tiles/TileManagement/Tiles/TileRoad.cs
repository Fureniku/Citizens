using System.Collections;
using System.Collections.Generic;
using Loading.States;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileRoad : TileData {

    [SerializeField] private RoadType roadType = RoadType.ROAD_STRAIGHT;
    
    void Start() {
        Initialize();
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

    public override void CreateFromRegistry() {
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
    CROSSING_APPROACH,
    ROAD_WORLD_EXIT,
    ROAD_WORLD_EDGE_STRAIGHT,
    ROAD_WORLD_EDGE_CORNER
}
