using Loading.States;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileBuildingSegment : TileData {

    [SerializeField] private GameObject mid;
    [SerializeField] private GameObject roof;
    [SerializeField] private float sectionHeight = 16.43f;
    [SerializeField] private float baseExtraHeight = 0.0f;

    private int segments = 5;

    private bool ready = false;
    private bool generated = false;
    
    void Start() { //Segments work differently to big buildings and are always 1x1
        width = 1;
        length = 1;
        sectionHeight *= World.Instance.GetChunkManager().GetWorldScale();
        baseExtraHeight *= World.Instance.GetChunkManager().GetWorldScale();
        Initialize();
    }
    
    public void MakeReady(int height) {
        this.segments = height;
        ready = true;
    }

    void Update() {
        if (ready && !generated) {
            Generate();
        }
    }

    void Generate() {
        Vector3 pos = transform.position;
        for (int i = 0; i < segments; i++) {
            GameObject generatedObj = null;
            if (i == segments-1) {
                if (roof != null) generatedObj = Instantiate(roof, new Vector3(pos.x, pos.y + baseExtraHeight + i*sectionHeight + roof.transform.position.y, pos.z), transform.rotation, transform);
            } else if (i != 0) {
                if (mid != null) generatedObj = Instantiate(mid, new Vector3(pos.x, pos.y + baseExtraHeight + i*sectionHeight + mid.transform.position.y, pos.z), transform.rotation, transform);
            }

            if (generatedObj != null) {
                generatedObj.name = gameObject.name + " (Floor " + i + ")";
                generatedObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        generated = true;
    }

    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetTilePos().x));
        jObj.Add(new JProperty("col", data.GetTilePos().z));
        
        JObject buildingObj = new JObject();
        //TODO
        //buildingObj.Add(new JProperty("segments", segments));
        //buildingObj.Add(new JProperty("segmentHeight", segmentHeight));
        //buildingObj.Add(new JProperty("height", height));
        buildingObj.Add(new JProperty("direction", genDirection.ToString()));

        jObj.Add(new JProperty("buildingTile", buildingObj));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }

    public override void DeserializeTile(JObject json) {
        SetId(ParseInt(json.GetValue("id")));
        SetName(tileName);
        SetRotation(Direction.GetDirection(ParseInt(json.GetValue("rotation"))));
        SetLocalPos(new LocalPos(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col"))));
        
        JObject buildingObj = (JObject) json.GetValue("buildingTile");
        if (buildingObj != null) {
            //TODO
            //segments = ParseInt(buildingObj.GetValue("segments"));
            //segmentHeight = ParseInt(buildingObj.GetValue("segmentHeight"));
            //height = ParseInt(buildingObj.GetValue("height"));
            genDirection = GenerateDirection.GetFromString(buildingObj.GetValue("direction").ToString());
        }
    }

    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
        //isRegistryEntry = true;
    }

    public override void CreateFromRegistry() {
        CreateBase();
        //isRegistryEntry = false;
    }
}