﻿using System.Collections;
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
        gridManager = GridManager.Instance;
        lastPos = TilePos.GetGridPosFromLocation(transform.position);
    }

    void Update() {
        if (gridManager.IsInitialized()) {
            if (roadGenStage == EnumGenerationStage.INITIALIZED) {
                //BeginRoadGeneration();
                roadGenStage = EnumGenerationStage.STARTED;
            }
        }
    }
    
    /*public void BeginRoadGeneration() {
        GenerateRoad(road_straight, TilePos.GetGridPosFromLocation(transform.position), generatorDirection);

        StartCoroutine(GeneratorCoroutine());
    }
    
    //Start generating the road
    IEnumerator GeneratorCoroutine() {
        //Loop for maximum road size
        for (int i = 0; i < numberGenerate; i++) {
            TilePos placePos = generatorDirection.OffsetPos(lastPos);
            if (gridManager.IsValidLocation(placePos)) {
                TileData tile = TileData.GetFromGameObject(gridManager.GetGridCellContents(placePos));
                float waitTime = 0f;
                GameObject placeTile = road_straight;
                if (tile != null) {
                    int existingId = tile.GetId();
                    EnumTileDirection placeRotation = generatorDirection;

                    if (tile is TileRoad) { //there's already a road here so we need to decide what to do.
                        Debug.Log("Existing tile is a road with ID " + existingId);
                        
                        if (placeTile.GetComponent<TileData>().GetId() != tile.GetId() || !tile.RotationMatch(generatorDirection)) {
                            Debug.Log("Replace! What type?");

                            if (existingId == road_straight.GetComponent<TileData>().GetId()) {
                                placeTile = road_t_junct;
                                placeRotation = generatorDirection.RotateCCW();
                            }

                            if (existingId == road_corner.GetComponent<TileData>().GetId()) {
                                placeTile = road_t_junct;
                                placeRotation = generatorDirection.Opposite();
                            }

                            if (existingId == road_crossroad.GetComponent<TileData>().GetId()) {
                                placeTile = road_crossroad_controlled;
                                placeRotation = generatorDirection;
                            }
                                
                            GenerateRoad(placeTile, placePos, placeRotation);
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
                                TileData tileRight = gridManager.GetGridCellContents(Direction.OffsetPos(generatorDirection.RotateCW(), placePos)).GetComponent<TileData>();
                                if (!(tileRight is TileGrass) && !(tileRight is TileRoad)) { //If there's something occupying the space to the right
                                    Debug.Log("[Right] Space is occupied. Try again later.");
                                    forceCorner = 1;
                                }
                                else {
                                    generatorDirection = generatorDirection.RotateCW();
                                    placeTile = road_corner;
                                    forceCorner = 0;
                                }
                            }
                            else {
                                TileData tileLeft = gridManager.GetGridCellContents(Direction.OffsetPos(generatorDirection.RotateCCW(), placePos)).GetComponent<TileData>();
                                if (!(tileLeft is TileGrass) && !(tileLeft is TileRoad)) { //If there's something occupying the space to the right
                                    Debug.Log("[Left] Space is occupied. Try again later.");
                                    forceCorner = 2;
                                }
                                else {
                                    generatorDirection = generatorDirection.RotateCCW();
                                    placeTile = road_corner;
                                    forceCorner = 0;
                                }
                            }
                            Debug.Log("Direction is now " + generatorDirection);

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

                TilePos ahead = Direction.OffsetPos(generatorDirection, placePos);
                if (gridManager.IsValidLocation(ahead)) {
                    GameObject tileAt = gridManager.GetGridCellContents(ahead);
                    Debug.Log("Tile ahead is: " + tileAt.GetComponent<TileData>().GetName());
                }
                GenerateRoad(placeTile, placePos, generatorDirection);
 
                lastPos = placePos;
                yield return new WaitForSeconds(waitTime);
            } else {
                Debug.Log("Invalid location. Time to handle that I guess.");
                break;
            }
        }

        Debug.Log("Road generation complete.");
        SaveLoadChunk.SerializeChunk();
        roadGenStage = EnumGenerationStage.COMPLETE;
        yield return null;
    }
    
    //Generate a road tile ready for placement
    private void GenerateRoad(GameObject type, TilePos pos, EnumTileDirection rot) {
        type.GetComponent<TileData>().SetRotation(rot);
        gridManager.FillGridCell(type, pos.x, pos.z, rot, false);
        Debug.Log("Road has been placed. Begin skyscraper placement.");
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
            //GenerateBuilding(skyscraper, right.x, right.z);
        }

        if (GenerateSkyscraperForPos(left, ref skyscraper)) {
            //GenerateBuilding(skyscraper, left.x, left.z);
        }
        Debug.Log("Generation should be complete.");
    }

    private void GenerateBuilding(GameObject building, int row, int col) {
        gridManager.FillGridCell(building, row, col, 0, true);
    }

    private bool GenerateSkyscraperForPos(TilePos pos, ref GameObject go) {
        EnumGenerateDirection skyscraperDir = gridManager.GetAvailableGenerateDirection(pos, go.GetComponent<TileData>());
        go.GetComponent<TileData>().SetGenerationDirection(skyscraperDir);
        Debug.Log("!!!!!!!!!!! Setting direction to " + skyscraperDir + " - this should be BEFORE any form of skyscraper generation!!");
        
        return skyscraperDir != EnumGenerateDirection.NONE; //return true if can generate, return false if not.
    }*/

    public bool IsGenerationComplete() {
        return roadGenStage == EnumGenerationStage.COMPLETE;
    }
}