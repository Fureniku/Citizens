using Tiles.TileManagement;
using UnityEngine;

public class RoadSeed : MonoBehaviour {
    
    private TilePos lastPos;
    private GridManager gridManager;
    
    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;

    [SerializeField] private GameObject roadGenerator = null;

    void Start() {
        gridManager = World.Instance.GetGridManager();
        float halfPos = (gridManager.GetGridTileSize() * World.Instance.GetGridManager().GetSize() * Chunk.size) / 2;
        transform.position = World.Instance.GetGridManager().transform.position + new Vector3(halfPos, 0, halfPos);
    }
    
    void Update() {
        if (gridManager.IsInitialized()) {
            if (roadGenStage == EnumGenerationStage.INITIALIZED) {
                BeginRoadGeneration();
                roadGenStage = EnumGenerationStage.STARTED;
            }
        }
    }
    
    public void BeginRoadGeneration() {
        GameObject road = TileRegistry.GetGameObjectFromID(TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetId());
        GenerateStartPoint(road, TilePos.GetGridPosFromLocation(transform.position));
    }
    
    public void GenerateStartPoint(GameObject type, TilePos pos) {
        EnumTileDirection rot = type.GetComponent<TileData>().GetRotation();
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        Chunk chunk = gridManager.GetChunk(chunkPos);
        chunk.FillChunkCell(type, LocalPos.FromTilePos(pos), rot, false);

        TilePos north = Direction.OffsetPos(EnumTileDirection.NORTH, pos);
        TilePos east  = Direction.OffsetPos(EnumTileDirection.EAST,  pos);
        TilePos south = Direction.OffsetPos(EnumTileDirection.SOUTH, pos);
        TilePos west  = Direction.OffsetPos(EnumTileDirection.WEST,  pos);

        GameObject genNorth = Instantiate(roadGenerator, TilePos.GetWorldPosFromTilePos(north), Quaternion.identity, transform);
        GameObject genEast  = Instantiate(roadGenerator, TilePos.GetWorldPosFromTilePos(east), Quaternion.identity, transform);
        GameObject genSouth = Instantiate(roadGenerator, TilePos.GetWorldPosFromTilePos(south), Quaternion.identity, transform);
        GameObject genWest  = Instantiate(roadGenerator, TilePos.GetWorldPosFromTilePos(west), Quaternion.identity, transform);

        genNorth.name = "North InitGen";
        genEast.name = "East InitGen";
        genSouth.name = "South InitGen";
        genWest.name = "West InitGen";

        genNorth.GetComponent<RoadGenerator>().SetDirection(EnumTileDirection.NORTH);
        genEast.GetComponent<RoadGenerator>().SetDirection(EnumTileDirection.EAST);
        genSouth.GetComponent<RoadGenerator>().SetDirection(EnumTileDirection.SOUTH);
        genWest.GetComponent<RoadGenerator>().SetDirection(EnumTileDirection.WEST);
    }
}