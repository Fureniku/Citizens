using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperGenerator : MonoBehaviour {

    [SerializeField] private GameObject baseSegment = null;
    [SerializeField] private GameObject midSegment = null;
    [SerializeField] private GameObject roofSegment = null;
    [SerializeField] private GameObject roadGen = null;

    private RoadGenerator roadGenerator;
    
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
        tilePos = TilePos.GetGridPosFromLocation(transform.position, gridManager);
        tileData.SetGridPos(tilePos);
        roadGenerator = roadGen.GetComponent<RoadGenerator>();

        //Debug.Log("Generating skyscraper with height " + height + ", " + segments + " segments and a scale of " + scale);
    }

    void Update() {
        if (gridManager.IsInitialized() && !skyscraperCreated && roadGenerator.IsGenerationComplete()) {
            Debug.Log("Attempting skyscraper generation");
            skyscraperCreated = true;
            if (CheckCanFit()) {
                Debug.Log("Enough space! Generating...");
                Generate();
            }
        }
    }

    private bool CheckCanFit() {
        bool[,] spaceCheck = new bool[gridLength,gridWidth]; 
        Debug.Log("Checking fit in " + gridLength + ", " + gridWidth);
        
        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridWidth; j++) {
                TilePos placeLoc = new TilePos(tilePos.x + j, tilePos.z + i);
                Debug.Log("Checking tile at " + placeLoc.x + ", " + placeLoc.z);
                if (gridManager.IsValidLocation(placeLoc)) {
                    GameObject goTile = gridManager.GetGridCellContents(placeLoc);
                    TileData tile = TileData.GetFromGameObject(goTile);
                    if (tile != null) {
                        Debug.Log("it's a "+ tile.GetName());
                        if (!(tile is TileGrass)) {
                            Debug.Log("Tile isn't grass. No space, aborting.");
                            return false;
                        }
                    }
                }
            }
        }
        
        return true;
    }
    
    private void Generate() {
        GameObject baseGO = Instantiate(baseSegment, transform);
        baseGO.transform.parent = transform;
        baseGO.transform.localScale = new Vector3(scale, 1.0f, scale);
        
        Vector3 pos = transform.position;

        int createdSegments = 0;

        for (int i = 0; i < segments; i++) {
            for (int j = 0; j < segmentHeight + (i == 0 ? height % segments : 0); j++) {
                GameObject go = Instantiate(midSegment, new Vector3(pos.x, pos.y + 8 + (createdSegments*4), pos.z), Quaternion.identity);
                go.transform.localScale = new Vector3(scale-(i*0.1f), 1, scale-(i*0.1f));
                go.transform.parent = transform;
                createdSegments++;
            }
        }

        GameObject roofGO = Instantiate(roofSegment, new Vector3(pos.x, pos.y + 8 + (createdSegments*4), pos.z), Quaternion.identity);
        roofGO.transform.parent = transform;
        roofGO.transform.localScale = new Vector3(scale-((segments-1)*0.1f), 1, scale-((segments-1)*0.1f));

        generationComplete = true;
    }

    public bool IsGenerationComplete() {
        return generationComplete;
    } 
}
