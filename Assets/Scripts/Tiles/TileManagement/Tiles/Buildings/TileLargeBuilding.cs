using Newtonsoft.Json.Linq;

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
        //isRegistryEntry = true;
    }

    public override void CreateFromRegistry() {
        CreateBase();

        Generate();
        //isRegistryEntry = false;
    }
}