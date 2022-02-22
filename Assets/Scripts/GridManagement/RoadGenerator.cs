using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {
    
    [SerializeField] private GameObject road_straight = null;
    [SerializeField] private GameObject road_corner = null;
    [SerializeField] private GameObject road_t_junct = null;
    [SerializeField] private GameObject road_crossroad = null;
    [SerializeField] private GameObject road_crossroad_controlled = null;
    
    [SerializeField] private GameObject skyscraper = null;

    [SerializeField] private int numberGenerate = 50;
    [SerializeField] private int cornerChance = 10; //as percentage
    [SerializeField] private int minTilesBeforeCorner = 4;

    [SerializeField] int generatorDirection = 0;
    private TilePos lastPos;
    private int tilesSinceCorner = 0;

    private GridManager gridManager;
    
    private float baseWeightTime = 0.0f; //for debugging

    private bool generationStarted = false;
    private bool generationComplete = false;
    
    void Start() {
        gridManager = GridManager.Instance;
        
        //TileRegistry.Register(road_straight.GetComponent<TileData>());
        //TileRegistry.Register(road_corner.GetComponent<TileData>());
        //TileRegistry.Register(road_t_junct.GetComponent<TileData>());
        //TileRegistry.Register(road_crossroad.GetComponent<TileData>());
        //TileRegistry.Register(road_crossroad_controlled.GetComponent<TileData>());
        
        lastPos = TilePos.GetGridPosFromLocation(transform.position);
    }

    void Update() {
        if (gridManager.IsInitialized()) {
            if (!generationStarted) {
                BeginRoadGeneration();
                generationStarted = true;
            }
        }
    }
    
    public void BeginRoadGeneration() {
        GenerateRoad(road_straight, TilePos.GetGridPosFromLocation(transform.position), generatorDirection);

        StartCoroutine(GeneratorCoroutine());
    }
    
    //Start generating the road
    IEnumerator GeneratorCoroutine() {
        //Loop for maximum road size
        for (int i = 0; i < numberGenerate; i++) {
            TilePos placePos = offsetPos(lastPos);
            if (gridManager.IsValidLocation(placePos)) {
                TileData tile = TileData.GetFromGameObject(gridManager.GetGridCellContents(placePos));
                float waitTime = 0f;
                if (tile != null) {
                    int existingId = tile.GetId();
                    GameObject placeTile = road_straight;
                    int placeRotation = generatorDirection;

                    if (tile is TileRoad) { //there's already a road here so we need to decide what to do.
                        Debug.Log("Existing tile is a road with ID " + existingId);
                        
                        if (placeTile.GetComponent<TileData>().GetId() != tile.GetId() || !tile.RotationMatch(generatorDirection)) {
                            Debug.Log("Replace! What type?");

                            if (existingId == road_straight.GetComponent<TileData>().GetId()) {
                                placeTile = road_t_junct;
                                placeRotation = generatorDirection + 90;
                            }

                            if (existingId == road_corner.GetComponent<TileData>().GetId()) {
                                placeTile = road_t_junct;
                                placeRotation = generatorDirection + 180;
                            }

                            if (existingId == road_crossroad.GetComponent<TileData>().GetId()) {
                                placeTile = road_crossroad_controlled;
                                placeRotation = generatorDirection;
                            }
                            
                            if (placeRotation >= 360) {
                                placeRotation -= 360;
                            }
                                
                            GenerateRoad(placeTile,  placePos,  placeRotation);
                            break;
                        }
                        
                    } else { //No road in the current position, we can continue generation.
                        //Check if we should randomly generate a corner
                        if (Random.Range(1, 100) <= cornerChance && tilesSinceCorner >= minTilesBeforeCorner) {
                            bool cornerRight = Random.value < 0.5;
                            waitTime = baseWeightTime * 4f;
                            if (cornerRight) {
                                GenerateRoad(road_corner, placePos, generatorDirection);
                                rotate(90);
                            }
                            else {
                                GenerateRoad(road_corner, placePos, generatorDirection + 90);
                                rotate(-90);
                            }

                            tilesSinceCorner = 0;
                        }
                        //Generate a straight road
                        else {
                            waitTime = baseWeightTime;
                            GenerateRoad(road_straight, placePos, generatorDirection);
                            tilesSinceCorner++;
                        }
                    }
                }
                else {
                    Debug.Log("Tile is null");
                    GenerateRoad(road_straight, placePos, generatorDirection);
                }

                lastPos = placePos;
                yield return new WaitForSeconds(waitTime);
            } else {
                Debug.Log("Invalid location. Time to handle that I guess.");
                break;
            }
        }

        Debug.Log("Road generation complete.");
        generationComplete = true;
        yield return null;
    }
    
    //Generate a road tile ready for placement
    private void GenerateRoad(GameObject type, TilePos pos, int rot) {
        gridManager.FillGridCell(type, pos.x, pos.z, rot, false);
        Debug.Log("Road has been placed. Begin skyscraper placement.");

            switch(rot) {
                case 0:
                    TilePos left  = new TilePos(pos.x - 1, pos.z);
                    TilePos right = new TilePos(pos.x + 1, pos.z);
                    if (GenerateSkyscraperForPos(right, ref skyscraper)) {
                        gridManager.FillGridCell(skyscraper, right.x, right.z, 0, true);
                    }

                    if (GenerateSkyscraperForPos(left, ref skyscraper)) {
                        gridManager.FillGridCell(skyscraper, left.x, left.z, 0, true);
                    }
                    break;
                case 90:
                    gridManager.FillGridCell(skyscraper, pos.x, pos.z + 1, 0, true);
                    //gridManager.FillGridCell(skyscraper, pos.x, pos.z - 1, 0, true);
                    break;
                case 180:
                    gridManager.FillGridCell(skyscraper, pos.x + 1, pos.z, 0, true);
                    //gridManager.FillGridCell(skyscraper, pos.x - 1, pos.z, 0, true);
                    break;
                case 270:
                    gridManager.FillGridCell(skyscraper, pos.x, pos.z + 1, 0, true);
                    //gridManager.FillGridCell(skyscraper, pos.x, pos.z - 1, 0, true);
                    break;
            }
            Debug.Log("Generation should be complete.");

    }

    private bool GenerateSkyscraperForPos(TilePos pos, ref GameObject go) {
        EnumGenerateDirection skyscraperDir = gridManager.GetAvailableGenerateDirection(pos, go.GetComponent<TileData>());
        go.GetComponent<TileData>().SetGenerationDirection(skyscraperDir);
        Debug.Log("!!!!!!!!!!! Setting direction to " + skyscraperDir + " - this should be BEFORE any form of skyscraper generation!!");
        
        return skyscraperDir != EnumGenerateDirection.NONE; //return true if can generate, return false if not.
    }

    //Calculate the direction to move based on rotation
    TilePos offsetPos(TilePos initialPos) {
        switch (generatorDirection) {
            case 0:
                initialPos.z -= 1;
                break;
            case 90:
                initialPos.x -= 1;
                break;
            case 180:
                initialPos.z += 1;
                break;
            case 270:
                initialPos.x += 1;
                break;
        }
        return initialPos;
    }

    //Rotate the generator, keeping within constraints to use elsewhere
    void rotate(int rot) {
        generatorDirection += rot;
        if (generatorDirection < 0) {
            generatorDirection += 360;
        }

        if (generatorDirection >= 360) {
            generatorDirection -= 360;
        }
    }

    public bool IsGenerationComplete() {
        return generationComplete;
    }
}
