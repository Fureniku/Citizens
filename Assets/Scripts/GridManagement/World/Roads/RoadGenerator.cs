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

    [ReadOnly, SerializeField] private int nestLevel = 0;
    
    [SerializeField] private GameObject skyscraper = null;

    [SerializeField] private int numberGenerate = 50;
    [SerializeField] private int cornerChance; //as percentage
    [SerializeField] private int minTilesBeforeCorner = 4;

    [SerializeField] EnumDirection generatorDirection = EnumDirection.SOUTH;
    
    private TilePos lastPos;
    [ReadOnly, SerializeField] private int tilesSinceBranch = 0;

    [ReadOnly, SerializeField] private int maxBranches = 5;
    [ReadOnly, SerializeField] private int branchesMade = 0;

    private ChunkManager chunkManager;
    private RoadSeed roadSeed = null;

    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;
    
    void Start() {
        chunkManager = World.Instance.GetChunkManager();
        lastPos = TilePos.GetGridPosFromLocation(transform.position);
        tilesSinceBranch = 0;
        nestLevel++;
        if (nestLevel > 1) {
            cornerChance /= 2; //Half corner chance each sublevel
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
                                
                            GenerateRoad(placeTile, placePos, placeRotation);
                            break;
                        }
                        
                    } else { //No road in the current position, we can continue generation.
                        //Check if we should randomly generate a corner
                        if ((Random.Range(1, 100) <= cornerChance && tilesSinceBranch >= minTilesBeforeCorner)) {
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
                        }
                        //Generate a straight road
                        else {
                            GenerateRoad(road_straight, placePos, placeRotation);
                            tilesSinceBranch++;
                        }
                    }
                }
                
                GenerateRoad(placeTile, placePos, placeRotation);
                lastPos = placePos;
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
        chunk.FillChunkCell(tile, LocalPos.FromTilePos(pos), rot, false);

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