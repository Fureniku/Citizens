﻿using Newtonsoft.Json.Linq;
using Tiles.TileManagement;

public class TileAir : TileData {
    
    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetLocalPos().x));
        jObj.Add(new JProperty("col", data.GetLocalPos().z));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }

    public override void DeserializeTile(JObject json) {
        SetId(ParseInt(json.GetValue("id")));
        SetName(tileName);
        SetLocalPos(new LocalPos(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col"))));
    }
}