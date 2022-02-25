using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileBuilding : TileData {

    [SerializeField] private GameObject baseSegment = null;
    [SerializeField] private GameObject midSegment = null;
    [SerializeField] private GameObject roofSegment = null;
    [SerializeField] private GameObject referenceTile = null;
    
    [SerializeField] private GameObject[,] referenceTiles;

    private bool generationComplete = false;
    private TileData tileData;

    private int segments;
    private int height;
    private int segmentHeight;
    private float scale;

    private int gridWidth;
    private int gridLength;

    private bool skyscraperCreated;

    private GridManager gridManager;
    private TilePos tilePos;

    
    
    void Start() {
        segments = Random.Range(1,3);
        height = Random.Range(30, 60);
        segmentHeight = height / segments;
        scale = Random.Range(0.85f, 1.2f);

        tileData = GetComponent<TileData>();

        gridWidth = tileData.GetWidth();
        gridLength = tileData.GetLength();
        
        gridManager = GridManager.Instance;
        tilePos = TilePos.GetGridPosFromLocation(transform.position);
        tileData.SetGridPos(tilePos);
        referenceTiles = new GameObject[gridWidth, gridLength];

        //Debug.Log("Generating skyscraper with height " + height + ", " + segments + " segments and a scale of " + scale);
    }

    void Update() {
        if (gridManager.IsInitialized() && !skyscraperCreated) {
            Debug.Log("######### Attempting skyscraper generation ###########");
            skyscraperCreated = true;
            if (genDirection != EnumGenerateDirection.NONE) {
                Debug.Log("Enough space! Generating in " + genDirection + ". Generating...");
                Generate();
            }
            else {
                Debug.Log("It all failed! Direction was " + genDirection);
            }
        }
    }

    private void Generate() {
        GameObject baseGO = Instantiate(baseSegment, transform);

        float scaleX = width;
        float scaleY = Mathf.Max(width, length) - (Mathf.Abs(width - length) / 2);
        float scaleZ = length;

        float xOffset = (1.5f * gridManager.GetGridTileSize()) * genDirection.GenX();
        float zOffset = (1.5f * gridManager.GetGridTileSize()) * genDirection.GenZ();
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

        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridWidth; j++) {
                if (i == 0 && j == 0) continue;
                //j and i * 1/* - 1
                Debug.Log("setting reference tiles. Generation is " + genDirection + " AKA " + genDirection.Name() + ", with X mod " + genDirection.GenX() + " and z mod " + genDirection.GenZ());
                gridManager.FillGridCell(referenceTile, tilePos.x + (j * genDirection.GenX()), tilePos.z + (i * genDirection.GenZ()), 0, false);
                GameObject rt = gridManager.GetGridCellContents(tilePos.x + j, tilePos.z + i);
                TileReference reference = rt.GetComponent<TileReference>();
                if (reference != null) {
                    rt.GetComponent<TileReference>().SetMasterTile(gameObject);
                }
                referenceTiles[j, i] = rt;
            }
        }

        generationComplete = true;
    }

    public bool IsGenerationComplete() {
        return generationComplete;
    }

    private void OnDestroy() {
        Debug.Log("Goodbye cruel world!");
        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridWidth; j++) {
                GameObject go = referenceTiles[j, i];
                if (go != null) {
                    Debug.Log("Setting " + (tilePos.z + i) + ", " + (tilePos.x + j) + ", to grass");
                    GameObject grassTile = TileRegistry.GetGrass();
                    gridManager.FillGridCell(grassTile, tilePos.x + j, tilePos.z + i, 0, false);
                }
            }
        }
    }

    public override JProperty SerializeTile(int row, int col) {
        TileData data = GridManager.Instance.GetGridTile(row, col);
            
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
