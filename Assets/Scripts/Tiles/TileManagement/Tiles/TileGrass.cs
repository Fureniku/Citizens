using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileGrass : TileData {
    
    void Start() {
        Initialize();
        width = 1;
        length = 1;
        halfRotations = false;
        rotation = EnumTileDirection.SOUTH;
    }
    
    public override JProperty SerializeTile(int row, int col) {
        TileData data = GridManager.Instance.GetGridTile(row, col);
            
        JObject jObj = new JObject();
        Debug.Log("Saving Grass with ID " + data.GetId());
        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetGridPos().x));
        jObj.Add(new JProperty("col", data.GetGridPos().z));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }

    public override void DeserializeTile(JObject json) {
        SetId(ParseInt(json.GetValue("id")));
        SetName(tileName);
        SetRowCol(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col")));
    }
}
