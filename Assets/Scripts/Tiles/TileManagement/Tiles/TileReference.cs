using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileReference : TileData {
    
    [SerializeField] private GameObject masterTile;

    public void SetMasterTile(GameObject tile) {
        masterTile = tile;
    }

    public GameObject GetMasterTile(GameObject tile) {
        return masterTile;
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
        SetRowCol(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col")));
    }
}
