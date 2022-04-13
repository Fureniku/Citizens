using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {
    
    private Tile road_straight = TileRegistry.STRAIGHT_ROAD_1x1;
    private Tile road_corner = TileRegistry.CORNER_ROAD_1x1;
    private Tile road_t_junct = TileRegistry.T_JUNCT_ROAD_1x1;
    private Tile road_crossroad = TileRegistry.CROSSROAD_ROAD_1x1;
    private Tile road_crossroad_controlled = TileRegistry.CROSSROAD_CTRL_ROAD_1x1;
    private Tile road_world_exit = TileRegistry.ROAD_WORLD_EXIT;
    private Tile road_zebra_crossing = TileRegistry.ZEBRA_CROSSING_1x1;
    private Tile road_pelican_crossing = TileRegistry.PELICAN_CROSSING_1x1;
    private Tile road_crossing_approach = TileRegistry.CROSSING_APPROACH_1x1;

    [ReadOnly, SerializeField] private int nestLevel = 0;

    [SerializeField] private int numberGenerate = 50;
    [SerializeField] private int junctionChance = 10; //as percentage
    [SerializeField] private int crossingChance = 20; //as percentage
    [SerializeField] private int minTilesBeforeJunction = 4;
    [SerializeField] private int minTilesBeforeCrossing = 2;

    [SerializeField] EnumDirection generatorDirection = EnumDirection.SOUTH;
    
    private TilePos lastPos;
    [ReadOnly, SerializeField] private int tilesSinceBranch = 0;
    [ReadOnly, SerializeField] private int tilesSinceCrossing = 0;

    [ReadOnly, SerializeField] private int maxBranches = 5;
    [ReadOnly, SerializeField] private int branchesMade = 0;

    private int crossingProgress = -1;

    private ChunkManager chunkManager;
    private RoadSeed roadSeed = null;

    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;
    
    void Start() {
        chunkManager = World.Instance.GetChunkManager();
        lastPos = TilePos.GetGridPosFromLocation(transform.position);
        tilesSinceBranch = 0;
        nestLevel++;
        if (nestLevel > 1) {
            junctionChance /= 2; //Half corner chance each sublevel
        }

        branchesMade = 0;
        if (maxBranches > 1) maxBranches--;

        roadSeed = FindObjectOfType<RoadSeed>();
    }

    void Update() {
        if (chunkManager.IsComplete()) {
            if (roadGenStage == EnumGenerationStage.INITIALIZED) {
                roadSeed.AddRoadGen();
                BeginRoadGeneration();
            }
        }
    }
    
    public void BeginRoadGeneration() {
        GenerateRoad(road_straight, TilePos.GetGridPosFromLocation(transform.position), generatorDirection);
        roadGenStage = EnumGenerationStage.IN_PROGRESS;
        StartCoroutine(GeneratorCoroutine());
    }
    
    //Start generating the road
    IEnumerator GeneratorCoroutine() {
        if (roadGenStage != EnumGenerationStage.IN_PROGRESS) {
            yield return null;
        }
        //Loop for maximum road size
        for (int i = 0; i < numberGenerate; i++) {
            TilePos placePos = generatorDirection.OffsetPos(lastPos);
            if (chunkManager.IsValidTile(placePos)) {
                
                if (crossingProgress > 0) {
                    Debug.Log("Crossing stage: " + crossingProgress);
                    if (crossingProgress == 1) {
                        GenerateRoad(road_zebra_crossing, placePos, generatorDirection);
                    }
                    if (crossingProgress == 2) {
                        GenerateRoad(road_crossing_approach, placePos, generatorDirection.Opposite());
                    }
                    crossingProgress++;
                    lastPos = placePos;
                    if (crossingProgress >= 3) {
                        crossingProgress = -1;
                    }
                } else {
                    TileData tile = chunkManager.GetTile(placePos);
                    Tile placeTile = road_straight;
                    EnumDirection placeRotation = generatorDirection;
                    if (tile != null) {
                        int existingId = tile.GetId();
                        if (tile is TileRoad) { //there's already a road here so we need to decide what to do.
                            if (placeTile.GetId() != tile.GetId() || !tile.RotationMatch(generatorDirection)) {
                                if (existingId == road_straight.GetId()) {
                                    placeTile = road_t_junct;
                                    placeRotation = generatorDirection.RotateCCW();
                                }
    
                                if (existingId == road_t_junct.GetId()) {
                                    placeTile = road_crossroad;
                                }
    
                                if (existingId == road_corner.GetId()) {
                                    placeTile = road_t_junct;
                                }
    
                                if (existingId == road_crossroad.GetId()) {
                                    placeTile = road_crossroad_controlled;
                                }

                                if (existingId == road_crossing_approach.GetId()) {
                                    placeTile = road_t_junct;
                                    placeRotation = generatorDirection.RotateCCW();
                                }
                                
                                if (existingId == road_zebra_crossing.GetId() || existingId == road_pelican_crossing.GetId()) {
                                    Debug.LogError("Overwriting crossing with junction - need to remove crossing approaches.");
                                    placeTile = road_t_junct;
                                    placeRotation = generatorDirection.RotateCCW();
                                }
                                    
                                GenerateRoad(placeTile, placePos, placeRotation);
                                break;
                            }
                            
                        } else { //No road in the current position, we can continue generation.
                            //Check if we should randomly generate a corner
    
                            bool genJunction = Random.Range(1, 100) <= junctionChance && tilesSinceBranch >= minTilesBeforeJunction;
                            bool genCrossing = Random.Range(1, 100) <= crossingChance && tilesSinceCrossing >= minTilesBeforeCrossing;
    
                            if (genJunction && genCrossing) { //Can't generate both, flip a coin to decide which.
                                genJunction = Random.Range(0.0f, 1.0f) <= 0.5f;
                                genCrossing = !genJunction;
                            }
                            
                            if (genJunction) {
                                //Check that there isn't a road in the next two tiles
                                bool canContinue = false;
                                TilePos ahead1 = Direction.OffsetPos(generatorDirection, placePos);
                                TilePos ahead2 = Direction.OffsetPos(generatorDirection, placePos, 2);
                                if (chunkManager.IsValidTile(ahead1) && chunkManager.IsValidTile(ahead2)) {
                                    TileData tileAt1 = chunkManager.GetTile(ahead1);
                                    TileData tileAt2 = chunkManager.GetTile(ahead2);
                                    if (!(tileAt1 is TileRoad || tileAt2 is TileRoad)) {
                                        canContinue = true;
                                    }
                                }
                                
                                if (canContinue && branchesMade < maxBranches) {
                                    placeTile = road_t_junct;
                                    GenerateBranch(Direction.OffsetPos(generatorDirection.RotateCCW(), placePos), generatorDirection.RotateCCW());
                                    tilesSinceBranch = 0;
                                }
                            } else if (genCrossing) {
                                Debug.Log("Attempt to generate crossing!");
                                //Check that there isn't a road in the next three tiles
                                bool canContinue = false;
                                TilePos ahead1 = Direction.OffsetPos(generatorDirection, placePos);
                                TilePos ahead2 = Direction.OffsetPos(generatorDirection, placePos, 2);
                                TilePos ahead3 = Direction.OffsetPos(generatorDirection, placePos, 3);
                                if (chunkManager.IsValidTile(ahead1) && chunkManager.IsValidTile(ahead2) && chunkManager.IsValidTile(ahead3)) {
                                    TileData tileAt1 = chunkManager.GetTile(ahead1);
                                    TileData tileAt2 = chunkManager.GetTile(ahead2);
                                    TileData tileAt3 = chunkManager.GetTile(ahead3);
                                    if (!(tileAt1 is TileRoad || tileAt2 is TileRoad || tileAt3 is TileRoad)) {
                                        canContinue = true;
                                    }
                                }
                                
                                if (canContinue) {
                                    Debug.Log("Successfully generating crossing!");
                                    placeTile = road_crossing_approach;
                                    tilesSinceCrossing = 0;
                                    crossingProgress = 1;
                                }
                            }
                            //Generate a straight road
                            else {
                                GenerateRoad(road_straight, placePos, placeRotation);
                                tilesSinceBranch++;
                                tilesSinceCrossing++;
                            }
                        }
                    }
                    
                    GenerateRoad(placeTile, placePos, placeRotation);
                    lastPos = placePos;
                }
                yield return null;
            } else {
                GenerateRoad(road_world_exit, lastPos, generatorDirection.Opposite());
                break;
            }
        }
        roadSeed.AddRoadGenComplete();
        roadGenStage = EnumGenerationStage.COMPLETE;
        yield return null;
    }

    private void GenerateBranch(TilePos currentPos, EnumDirection direction) {
        GameObject branch = Instantiate(gameObject, TilePos.GetWorldPosFromTilePos(currentPos), Quaternion.identity, transform);
        foreach (Transform child in branch.transform) {
            Destroy(child.gameObject);
        }
        branch.name = "Branch Gen";
        branch.GetComponent<RoadGenerator>().SetDirection(direction);
        branchesMade++;
    }
    
    //Generate a road tile ready for placement
    private void GenerateRoad(Tile tile, TilePos pos, EnumDirection rot) {
        pos = TilePos.Clamp(pos);
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        Chunk chunk = chunkManager.GetChunk(chunkPos);
        if (!chunk.gameObject.activeSelf) {
            Debug.Log("Chunk is inactive! Enabling!");
            chunk.gameObject.SetActive(true);
        }
        chunk.FillChunkCell(tile, LocalPos.FromTilePos(pos), rot);

        //Debug.Log("Road has been placed. Begin skyscraper placement.");
        /*TilePos left  = new TilePos(pos.x, pos.z);
        TilePos right = new TilePos(pos.x, pos.z);
        switch(rot) {
            case EnumTileDirection.NORTH:
                left  = new TilePos(pos.x - 1, pos.z);
                right = new TilePos(pos.x + 1, pos.z);
                break;
            case EnumTileDirection.EAST:
                left  = new TilePos(pos.x, pos.z - 1);
                right = new TilePos(pos.x, pos.z + 1);
                break;
            case EnumTileDirection.SOUTH:
                left  = new TilePos(pos.x + 1, pos.z);
                right = new TilePos(pos.x - 1, pos.z);
                break;
            case EnumTileDirection.WEST:
                left  = new TilePos(pos.x, pos.z + 1);
                right = new TilePos(pos.x, pos.z - 1);
                break;
        }
        
        if (GenerateSkyscraperForPos(right, ref skyscraper)) {
            GenerateBuilding(skyscraper, right, chunk);
        }

        if (GenerateSkyscraperForPos(left, ref skyscraper)) {
            GenerateBuilding(skyscraper, left, chunk);
        }
        //Debug.Log("Generation should be complete.");*/
    }

    private void GenerateBuilding(GameObject building, TilePos pos, Chunk chunk) {
        //chunk.FillChunkCell(building, LocalPos.FromTilePos(pos), 0, true);
    }

    private bool GenerateSkyscraperForPos(TilePos pos, ref GameObject go) {
        EnumGenerateDirection skyscraperDir = chunkManager.GetAvailableGenerateDirection(pos, go.GetComponent<TileData>());
        go.GetComponent<TileData>().SetGenerationDirection(skyscraperDir);
        
        return skyscraperDir != EnumGenerateDirection.NONE; //return true if can generate, return false if not.
    }

    public bool IsGenerationComplete() {
        return roadGenStage == EnumGenerationStage.COMPLETE;
    }

    public void SetDirection(EnumDirection dir) {
        generatorDirection = dir;
    }

    public void IncrementNestLevel(int current) {
        nestLevel = current+1;
    }
}