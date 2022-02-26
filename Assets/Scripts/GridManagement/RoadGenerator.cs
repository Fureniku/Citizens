using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
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

    [SerializeField] EnumTileDirection generatorDirection = EnumTileDirection.SOUTH;
    
    private TilePos lastPos;
    private int tilesSinceCorner = 0;

    private GridManager gridManager;
    
    private float baseWeightTime = 0.0f; //for debugging

    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;

    private int forceCorner = 0; //A corner was attempted to generate, but a building occupies the space. 0 = no force, 1 = force right, 2 = force left
    
    void Start() {
        gridManager = World.Instance.GetGridManager();
        lastPos = TilePos.GetGridPosFromLocation(transform.position);
    }

    void Update() {
        if (gridManager.IsInitialized()) {
            if (roadGenStage == EnumGenerationStage.INITIALIZED) {
                BeginRoadGeneration();
                roadGenStage = EnumGenerationStage.STARTED;
            }
        }

        if (roadGenStage == EnumGenerationStage.DIRTY) {
            Debug.Log("Triggering save");
            gridManager.TriggerSave();
            roadGenStage = EnumGenerationStage.COMPLETE;
        }
    }
    
    public void BeginRoadGeneration() {
        road_straight.GetComponent<TileData>().SetRotation(generatorDirection);
        GenerateRoad(road_straight, TilePos.GetGridPosFromLocation(transform.position));

        StartCoroutine(GeneratorCoroutine());
    }
    
    //Start generating the road
    IEnumerator GeneratorCoroutine() {
        //Loop for maximum road size
        for (int i = 0; i < numberGenerate; i++) {
            TilePos placePos = generatorDirection.OffsetPos(lastPos);
            if (gridManager.IsValidTile(placePos)) {
                TileData tile = gridManager.GetTile(placePos);
                float waitTime = 0f;
                GameObject placeTile = road_straight;
                placeTile.GetComponent<TileData>().SetRotation(generatorDirection);
                if (tile != null) {
                    int existingId = tile.GetId();

                    if (tile is TileRoad) { //there's already a road here so we need to decide what to do.
                        Debug.Log("Existing tile is a road with ID " + existingId);
                        
                        if (placeTile.GetComponent<TileData>().GetId() != tile.GetId() || !tile.RotationMatch(generatorDirection)) {
                            Debug.Log("Replace! What type?");

                            if (existingId == road_straight.GetComponent<TileData>().GetId()) {
                                placeTile = road_t_junct;
                            }

                            if (existingId == road_corner.GetComponent<TileData>().GetId()) {
                                placeTile = road_t_junct;
                                placeTile.GetComponent<TileData>().SetRotation(generatorDirection);
                            }

                            if (existingId == road_crossroad.GetComponent<TileData>().GetId()) {
                                placeTile = road_crossroad_controlled;
                                placeTile.GetComponent<TileData>().SetRotation(generatorDirection);
                            }
                                
                            GenerateRoad(placeTile, placePos);
                            break;
                        }
                        
                    } else { //No road in the current position, we can continue generation.
                        //Check if we should randomly generate a corner
                        if ((Random.Range(1, 100) <= cornerChance && tilesSinceCorner >= minTilesBeforeCorner) || forceCorner > 0) {
                            if (forceCorner > 0) Debug.Log("A previous corner was requested but could not be placed. Trying again!");
                            bool cornerRight = forceCorner > 0 ? forceCorner == 1 : Random.value < 0.5;
                            waitTime = baseWeightTime * 4f;
                            Debug.Log("Turning!");
                            if (cornerRight) {
                                TileData tileRight = gridManager.GetTile(Direction.OffsetPos(generatorDirection.RotateCW(), placePos)).GetComponent<TileData>();
                                if (!(tileRight is TileGrass) && !(tileRight is TileRoad)) { //If there's something occupying the space to the right
                                    Debug.Log("[Right] Space is occupied. Try again later.");
                                    forceCorner = 1;
                                }
                                else {
                                    
                                    placeTile = road_corner;
                                    placeTile.GetComponent<TileData>().SetRotation(generatorDirection.Opposite());
                                    generatorDirection = generatorDirection.RotateCW();
                                    forceCorner = 0;
                                }
                            }
                            else {
                                TileData tileLeft = gridManager.GetTile(Direction.OffsetPos(generatorDirection.RotateCCW(), placePos)).GetComponent<TileData>();
                                if (!(tileLeft is TileGrass) && !(tileLeft is TileRoad)) { //If there's something occupying the space to the right
                                    Debug.Log("[Left] Space is occupied. Try again later.");
                                    forceCorner = 2;
                                }
                                else {
                                    placeTile = road_corner;
                                    placeTile.GetComponent<TileData>().SetRotation(generatorDirection.RotateCCW());
                                    generatorDirection = generatorDirection.RotateCCW();
                                    forceCorner = 0;
                                }
                            }
                            Debug.Log("generator direction is now " + generatorDirection);

                            tilesSinceCorner = 0;
                        }
                        //Generate a straight road
                        else {
                            waitTime = baseWeightTime;
                            GenerateRoad(road_straight, placePos);
                            tilesSinceCorner++;
                        }
                    }
                }

                TilePos ahead = Direction.OffsetPos(generatorDirection, placePos);
                if (gridManager.IsValidTile(ahead)) {
                    TileData tileAt = gridManager.GetTile(ahead);
                    if (tileAt != null) {
                        Debug.Log("Tile ahead is: " + tileAt.GetName());
                    }
                }
                GenerateRoad(placeTile, placePos);
 
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
    
    //Generate a road tile ready for placement
    private void GenerateRoad(GameObject type, TilePos pos) {
        EnumTileDirection rot = type.GetComponent<TileData>().GetRotation();
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        Chunk chunk = gridManager.GetChunk(chunkPos);
        chunk.FillChunkCell(type, LocalPos.FromTilePos(pos), rot, false);

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
        chunk.FillChunkCell(building, LocalPos.FromTilePos(pos), 0, true);
    }

    private bool GenerateSkyscraperForPos(TilePos pos, ref GameObject go) {
        EnumGenerateDirection skyscraperDir = gridManager.GetAvailableGenerateDirection(pos, go.GetComponent<TileData>());
        go.GetComponent<TileData>().SetGenerationDirection(skyscraperDir);
        
        return skyscraperDir != EnumGenerateDirection.NONE; //return true if can generate, return false if not.
    }

    public bool IsGenerationComplete() {
        return roadGenStage == EnumGenerationStage.COMPLETE;
    }
}