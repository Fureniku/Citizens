using Newtonsoft.Json.Linq;
using Tiles.TileManagement;

public class TileLargeBuilding : TileData {
    
    void Start() {
        Initialize();
    }
    
    void Generate() {

    }

    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();
        return new JProperty($"tile_{row}_{col}", jObj);
    }
    
    public override void DeserializeTile(JObject json) {
    }
    
    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
    }

    public override void CreateFromRegistry() {
        CreateBase();

        Generate();
    }
    
    public override void UpdateTile() {}
    public override void OnNeighbourChanged(EnumDirection neighbour) {}
}