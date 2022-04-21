using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileGrass : TileData {
    [SerializeField] private bool inSection = false;
    
    void Start() {
        Initialize();
        width = 1;
        length = 1;
        halfRotations = false;
        rotation = EnumDirection.SOUTH;
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
        SetLocalPos(new LocalPos(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col"))));
    }
    
    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
    }

    public override void CreateFromRegistry() {
        CreateBase();
    }

    public bool IsInSection() {
        return inSection;
    }

    public void AddToSection() => inSection = true;
}
