using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGenericSkyscraper : TileBuilding {

    [SerializeField] private GameObject baseSegment = null;
    [SerializeField] private GameObject midSegment = null;
    [SerializeField] private GameObject roofSegment = null;
    [SerializeField] private GameObject referenceTile = null;

    [SerializeField] private int minSegments = 1;
    [SerializeField] private int maxSegments = 3;

    [ReadOnly, SerializeField] private int segments = 1;
    [ReadOnly, SerializeField] private int height = 30;
    
    private GameObject[,] referenceTiles;

    private bool generationComplete = false;

    private int segmentHeight;
    private float scale;

    private bool skyscraperCreated;
    private bool isRegistryEntry = false;

    void Start() {
        segments = Random.Range(minSegments, maxSegments);
        height = Random.Range(minHeight, maxHeight);
        segmentHeight = height / segments;
        scale = Random.Range(0.85f, 1.2f);
        
        Initialize();
        SetInitialPos();
        referenceTiles = new GameObject[width, length];

        //Debug.Log("Generating skyscraper with height " + height + ", " + segments + " segments and a scale of " + scale);
    }

    void Update() {
        if (World.Instance.GetChunkManager().IsComplete() && !skyscraperCreated) {
            skyscraperCreated = true;
            if (genDirection != EnumGenerateDirection.NONE && !isRegistryEntry) {
                Generate();
            }
        }
    }

    private void Generate() {
        GameObject baseGO = Instantiate(baseSegment, transform);

        float scaleX = width;
        float scaleY = Mathf.Max(width, length) - (Mathf.Abs(width - length) / 2);
        float scaleZ = length;

        float xOffset = (1.5f * World.Instance.GetChunkManager().GetGridTileSize()) * genDirection.GenX();
        float zOffset = (1.5f * World.Instance.GetChunkManager().GetGridTileSize()) * genDirection.GenZ();
        Vector3 pos = transform.position;
        baseGO.transform.parent = transform;
        baseGO.transform.position = new Vector3(pos.x + xOffset, pos.y, pos.z + zOffset);
        baseGO.transform.localScale = new Vector3(scale * scaleX, 1.0f*scaleY, scale * scaleZ);

        int createdSegments = 0;

        for (int i = 0; i < segments; i++) {
            for (int j = 0; j < segmentHeight + (i == 0 ? height % segments : 0); j++) {
                GameObject go = Instantiate(midSegment, new Vector3(pos.x + xOffset, pos.y + ((8 + (createdSegments*4))*scaleY), pos.z + zOffset), Quaternion.identity);
                go.transform.localScale = new Vector3((scale-(i*0.1f)) * scaleX, 1*scaleY, (scale-(i*0.1f)) * scaleZ);
                go.transform.parent = transform;
                createdSegments++;
            }
        }

        GameObject roofGO = Instantiate(roofSegment, new Vector3(pos.x + xOffset, pos.y + ((8 + (createdSegments*4))*scaleY), pos.z + zOffset), Quaternion.identity);
        roofGO.transform.parent = transform;
        roofGO.transform.localScale = new Vector3((scale-((segments-1)*0.1f)) * scaleX, 1*scaleY, (scale-((segments-1)*0.1f)) * scaleZ);

        //Place reference tiles
        for (int row = 0; row < length; row++) {
            for (int col = 0; col < width; col++) {
                if (row == 0 && col == 0) continue;
                TilePos genPos = new TilePos(worldPos.x + row * genDirection.GenX(), worldPos.z + col * genDirection.GenZ());
                Chunk chunk = World.Instance.GetChunkManager().GetChunk(TilePos.GetParentChunk(genPos));
                LocalPos lp = LocalPos.FromTilePos(genPos);
                chunk.FillChunkCell(referenceTile, lp, 0);
                GameObject rt = chunk.GetChunkCellContents(lp.x, lp.z);
                TileReference reference = rt.GetComponent<TileReference>();
                if (reference != null) {
                    rt.GetComponent<TileReference>().SetMasterTile(TilePos.GetGridPosFromLocation(gameObject.transform.position));
                }
                referenceTiles[row, col] = rt;
                    
            }
        }
        generationComplete = true;
    }

    public bool IsGenerationComplete() {
        return generationComplete;
    }

    private void OnDestroy() {
        for (int row = 0; row < length; row++) {
            for (int col = 0; col < width; col++) {
                GameObject go = referenceTiles[row, col];
                if (go != null) {
                    Debug.Log("Setting " + (worldPos.x + row) + ", " + (worldPos.z + col) + ", to grass");
                    GameObject grassTile = TileRegistry.GetGrass();
                    Debug.Log("Not actually setting right now! Fix the function!!");
                    //gridManager.FillGridCell(grassTile, tilePos.x + j, tilePos.z + i, 0, false);
                }
            }
        }
    }

    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetGridPos().x));
        jObj.Add(new JProperty("col", data.GetGridPos().z));
        
        JObject buildingObj = new JObject();
        
        buildingObj.Add(new JProperty("segments", segments));
        buildingObj.Add(new JProperty("segmentHeight", segmentHeight));
        buildingObj.Add(new JProperty("height", height));
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
            segments = ParseInt(buildingObj.GetValue("segments"));
            segmentHeight = ParseInt(buildingObj.GetValue("segmentHeight"));
            height = ParseInt(buildingObj.GetValue("height"));
            genDirection = GenerateDirection.GetFromString(buildingObj.GetValue("direction").ToString());
        }
    }

    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
        isRegistryEntry = true;
    }

    public override void Create() {
        CreateBase();
        isRegistryEntry = false;
    }
}