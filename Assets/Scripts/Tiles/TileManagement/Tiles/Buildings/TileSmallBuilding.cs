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

    private bool ready = false;
    private bool generated = false;
    
    void Start() { //Segments work differently to big buildings and are always 1x1
        width = 1;
        length = 1;
        
        SetMaterialToObject(materialObject1, materials1);
        SetMaterialToObject(materialObject2, materials2);
        SetMaterialToObject(materialObject3, materials3);
        SetMaterialToObject(materialObject4, materials4);

        Initialize();
    }

    private void SetMaterialToObject(GameObject objectIn, Material[] matsIn) {
        if (objectIn != null) {
            Material selectedMat = matsIn[matsIn.Length-1];
            MeshRenderer rendererIn = objectIn.GetComponent<MeshRenderer>();
            
            Debug.Log("setting " + objectIn.name + " material to  " + selectedMat.name);
            
            for (int i = 0; i < rendererIn.materials.Length; i++) {
                rendererIn.materials[i] = selectedMat;
            }
        }
    }
    
    public void MakeReady() {
        ready = true;
    }

    void Update() {
        if (ready && !generated) {
            Generate();
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
        
        SetMaterialToObject(materialObject1, materials1);
        SetMaterialToObject(materialObject2, materials2);

        generated = true;
    }

    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetGridPos().x));
        jObj.Add(new JProperty("col", data.GetGridPos().z));
        
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

    public override void Create() {
        CreateBase();
        //isRegistryEntry = false;
    }
}