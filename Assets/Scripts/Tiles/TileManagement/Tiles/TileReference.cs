using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileReference : TileData {
    
    [SerializeField] private TilePos masterTile;

    void Start() {
        Initialize();
    }

    public void SetMasterTile(TilePos tilePos) {
        masterTile = tilePos;
    }

    public TilePos GetMasterTile() {
        return masterTile;
    }
    
    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetGridPos().x));
        jObj.Add(new JProperty("col", data.GetGridPos().z));

        JObject referenceObj = new JObject();
        
        referenceObj.Add(new JProperty("masterX", masterTile.x));
        referenceObj.Add(new JProperty("masterZ", masterTile.z));

        jObj.Add(new JProperty("referenceTile", referenceObj));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }

    public override void DeserializeTile(JObject json) {
        SetId(ParseInt(json.GetValue("id")));
        SetName(tileName);
        SetRotation(Direction.GetDirection(ParseInt(json.GetValue("rotation"))));
        SetLocalPos(new LocalPos(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col"))));

        JObject referenceObj = (JObject) json.GetValue("referenceTile");
        if (referenceObj != null) {
            SetMasterTile(new TilePos(ParseInt(referenceObj.GetValue("masterX")), ParseInt(referenceObj.GetValue("masterZ"))));
        }
        
    }
}
