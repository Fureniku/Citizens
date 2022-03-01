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
        
        IndividualPoint(EnumTileDirection.NORTH, pos);
        IndividualPoint(EnumTileDirection.EAST,  pos);
        IndividualPoint(EnumTileDirection.SOUTH, pos);
        IndividualPoint(EnumTileDirection.WEST,  pos);
    }

    public void IndividualPoint(EnumTileDirection dir, TilePos currentPos) {
        TilePos startPos = Direction.OffsetPos(dir, currentPos);
        GameObject genNorth = Instantiate(roadGenerator, TilePos.GetWorldPosFromTilePos(startPos), Quaternion.identity, transform);
        genNorth.name = dir + " InitGen";
        genNorth.GetComponent<RoadGenerator>().SetDirection(dir);
    }
}