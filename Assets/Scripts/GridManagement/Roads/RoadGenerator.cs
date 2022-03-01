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

    [ReadOnly, SerializeField] private int nestLevel = 0;
    
    [SerializeField] private GameObject skyscraper = null;

    [SerializeField] private int numberGenerate = 50;
    [SerializeField] private int cornerChance; //as percentage
    [SerializeField] private int minTilesBeforeCorner = 4;

    [SerializeField] EnumTileDirection generatorDirection = EnumTileDirection.SOUTH;
    
    private TilePos lastPos;
    [ReadOnly, SerializeField] private int tilesSinceBranch = 0;

    [ReadOnly, SerializeField] private int maxBranches = 5;
    [ReadOnly, SerializeField] private int branchesMade = 0;

    private GridManager gridManager;
    
    private float baseWaitTime = 0.0f; //for debugging

    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;
    
    void Start() {
        gridManager = World.Instance.GetGridManager();
        lastPos = TilePos.GetGridPosFromLocation(transform.position);
        tilesSinceBranch = 0;
        nestLevel++;
        if (nestLevel > 1) {
            cornerChance /= 2; //Half corner chance each sublevel
        }

        branchesMade = 0;
        if (maxBranches > 1) maxBranches--;
    }

    void Update() {
        if (gridManager.IsInitialized()) {
            if (roadGenStage == EnumGenerationStage.INITIALIZED) {
                BeginRoadGeneration();
            }
        }

        if (roadGenStage == EnumGenerationStage.DIRTY) {
            Debug.Log("Triggering save");
            gridManager.TriggerSave();
            roadGenStage = EnumGenerationStage.COMPLETE;
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
            if (gridManager.IsValidTile(placePos)) {
                TileData tile = gridManager.GetTile(placePos);
                float waitTime = 0f;
                Tile placeTile = road_straight;
                EnumTileDirection placeRotation = generatorDirection;
                if (tile != null) {
                    int existingId = tile.GetId();
                    if (tile is TileRoad) { //there's already a road here so we need to decide what to do.
                        Debug.Log("Existing tile is a road with ID " + existingId);
                        
                        if (placeTile.GetId() != tile.GetId() || !tile.RotationMatch(generatorDirection)) {
                            Debug.Log("Replace! What type?");

                            if (existingId == road_straight.GetId()) {
                                placeTile = road_t_junct;
                                placeRotation = generatorDirection.RotateCCW();
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
                            if (gridManager.IsValidTile(ahead1) && gridManager.IsValidTile(ahead2)) {
                                TileData tileAt1 = gridManager.GetTile(ahead1);
                                TileData tileAt2 = gridManager.GetTile(ahead2);
                                if (!(tileAt1 is TileRoad || tileAt2 is TileRoad)) {
                                    canContinue = true;
                                }
                            }
                            
                            if (canContinue && branchesMade < maxBranches) {
                                bool cornerRight = false;
                                waitTime = baseWaitTime * 4f;
                                Debug.Log("Creating branch!");
                                if (cornerRight) {
                                    placeTile = road_t_junct;
                                    placeRotation = generatorDirection.Opposite();
                                    GenerateBranch(Direction.OffsetPos(generatorDirection.RotateCW(), placePos), generatorDirection.RotateCW());
                                }
                                else {
                                    placeTile = road_t_junct;
                                    GenerateBranch(Direction.OffsetPos(generatorDirection.RotateCCW(), placePos), generatorDirection.RotateCCW());
                                }

                                tilesSinceBranch = 0;
                            }
                        }
                        //Generate a straight road
                        else {
                            waitTime = baseWaitTime;
                            GenerateRoad(road_straight, placePos, placeRotation);
                            tilesSinceBranch++;
                        }
                    }
                }
                
                GenerateRoad(placeTile, placePos, placeRotation);
 
                lastPos = placePos;
                yield return new WaitForSeconds(waitTime);
            } else {
                Debug.Log("Invalid location. Time to handle that I guess.");
                break;
            }
        }

        Debug.Log("Road generation complete.");
        roadGenStage = EnumGenerationStage.DIRTY;
        yield return null;
    }

    private void GenerateBranch(TilePos currentPos, EnumTileDirection direction) {
        TilePos startPos = Direction.OffsetPos(direction, currentPos);
        GameObject branch = Instantiate(gameObject, TilePos.GetWorldPosFromTilePos(currentPos), Quaternion.identity, transform);
        foreach (Transform child in branch.transform) {
            Destroy(child.gameObject);
        }
        branch.name = "Branch Gen";
        branch.GetComponent<RoadGenerator>().SetDirection(direction);
        branchesMade++;
        //branch.GetComponent<RoadGenerator>().IncrementNestLevel(nestLevel);
    }
    
    //Generate a road tile ready for placement
    private void GenerateRoad(Tile tile, TilePos pos, EnumTileDirection rot) {
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        Chunk chunk = gridManager.GetChunk(chunkPos);
        chunk.FillChunkCell(tile, LocalPos.FromTilePos(pos), rot, false);

        //Debug.Log("Road has been placed. Begin skyscraper placement.");
        TilePos left  = new TilePos(pos.x, pos.z);
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
        //Debug.Log("Generation should be complete.");
    }

    private void GenerateBuilding(GameObject building, TilePos pos, Chunk chunk) {
        //chunk.FillChunkCell(building, LocalPos.FromTilePos(pos), 0, true);
    }

    private bool GenerateSkyscraperForPos(TilePos pos, ref GameObject go) {
        EnumGenerateDirection skyscraperDir = gridManager.GetAvailableGenerateDirection(pos, go.GetComponent<TileData>());
        go.GetComponent<TileData>().SetGenerationDirection(skyscraperDir);
        
        return skyscraperDir != EnumGenerateDirection.NONE; //return true if can generate, return false if not.
    }

    public bool IsGenerationComplete() {
        return roadGenStage == EnumGenerationStage.COMPLETE;
    }

    public void SetDirection(EnumTileDirection dir) {
        generatorDirection = dir;
    }

    public void IncrementNestLevel(int current) {
        nestLevel = current+1;
    }
}