using Loading.States;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileSmallBuilding : TileData {

    [SerializeField] private GameObject roof_full;
    [SerializeField] private GameObject roof_left;
    [SerializeField] private GameObject roof_right;
    [SerializeField] private GameObject roof_none;

    [SerializeField] private Material[] materials1;
    [SerializeField] private Material[] materials2;
    [SerializeField] private Material[] materials3;
    [SerializeField] private Material[] materials4;

    [SerializeField] private GameObject materialObject1;
    [SerializeField] private GameObject materialObject2;
    [SerializeField] private GameObject materialObject3;
    [SerializeField] private GameObject materialObject4;

    void Start() { //Segments work differently to big buildings and are always 1x1
        width = 1;
        length = 1;

        Initialize();
    }

    private void SetMaterialToObject(GameObject objectIn, Material[] matsIn) {
        if (objectIn != null) {
            Material selectedMat = matsIn[Random.Range(0, matsIn.Length)];
            MeshRenderer rendererIn = objectIn.GetComponent<MeshRenderer>();

            if (rendererIn.materials.Length > 1) {
                for (int i = 0; i < rendererIn.materials.Length; i++) {
                    rendererIn.materials[i] = selectedMat;
                }
            } else {
                rendererIn.material = selectedMat;
            }
        }
    }

    void Generate() {
        Vector3 pos = transform.position;
        GameObject roof = null;
        //TODO select roof type
        roof = Instantiate(roof_full, new Vector3(pos.x, pos.y, pos.z), transform.rotation, transform);
        
        if (roof != null) {
            roof.name = gameObject.name + " Roof";
            roof.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
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
    }

    public override void CreateFromRegistry() {
        CreateBase();
        
        SetMaterialToObject(materialObject1, materials1);
        SetMaterialToObject(materialObject2, materials2);
        SetMaterialToObject(materialObject3, materials3);
        SetMaterialToObject(materialObject4, materials4);

        Generate();
    }
    
    public override void UpdateTile() {}
    public override void OnNeighbourChanged(EnumDirection neighbour) {}
}