using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

    [SerializeField] private GameObject road_straight;
    [SerializeField] private GameObject road_corner;
    [SerializeField] private GameObject road_t_junct;
    [SerializeField] private GameObject road_crossroad;
    [SerializeField] private GameObject road_crossroad_controlled;

    [SerializeField] private int numberGenerate = 50;
    [SerializeField] private int cornerChance = 10; //as percentage
    [SerializeField] private int minTilesBeforeCorner = 4;

    [SerializeField] int generatorDirection = 0;
    private Vector3 lastPos;
    private int tilesSinceCorner = 0;
    
    private WorldGenerator worldGenerator;

    private float waitTime = 0.05f;
    
    void Start() {
        worldGenerator = WorldGenerator.Instance;
        
        TileRegistry.Register(road_straight.GetComponent<TileData>());
        TileRegistry.Register(road_corner.GetComponent<TileData>());
        TileRegistry.Register(road_t_junct.GetComponent<TileData>());
        TileRegistry.Register(road_crossroad.GetComponent<TileData>());
        TileRegistry.Register(road_crossroad_controlled.GetComponent<TileData>());
        
        lastPos = transform.position;
    }

    public void BeginRoadGeneration() {
        GenerateRoad(road_straight, new Vector3(lastPos.x,  lastPos.y, lastPos.z), generatorDirection);

        StartCoroutine(GeneratorCoroutine());
    }

    IEnumerator CheckValidityAfterDelay() {
        Debug.Log("Checking validity in 5 seconds...");
        yield return new WaitForSeconds(5.0f);
        TileGrid.CheckMapValidity();
    }

    //Start generating the road
    IEnumerator GeneratorCoroutine() {
        //Loop for maximum road size
        for (int i = 0; i < numberGenerate; i++) {
            Vector3 placePos = lastPos + offsetDir(generatorDirection);
            if (TileGrid.IsValidLocation(new TilePos(placePos))) {
                
                GameObject tileAtPos = TileGrid.GetGameObjectAtPos(placePos);
                TileData tile = TileData.GetFromGameObject(TileGrid.GetGameObjectAtPos(placePos));
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
                            waitTime = 1.0f;
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
                            waitTime = 0.05f;
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
    }

    //Generate a road tile ready for placement
    private void GenerateRoad(GameObject type, Vector3 vec3, int rot) {
        TilePos pos = new TilePos(vec3);

        worldGenerator.PlaceTile(type, vec3, rot, pos);
    }

    //Calculate the direction to move based on rotation
    Vector3 offsetDir(int rotation) {
        switch (rotation) {
            case 0:
                return new Vector3(0, 0, -10);
            case 90:
                return new Vector3(-10, 0, 0);
            case 180:
                return new Vector3(0, 0, 10);
            case 270:
                return new Vector3(10, 0, 0);
        }
        return new Vector3(0, 0, -10);
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
}
