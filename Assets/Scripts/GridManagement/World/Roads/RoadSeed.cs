using Tiles.TileManagement;
using UnityEngine;

public class RoadSeed : GenerationSystem {
    
    private GridManager gridManager;
    
    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;

    [SerializeField] private GameObject roadGenerator = null;
    [ReadOnly, SerializeField] private int roadGeneratorInstances = 0;
    [ReadOnly, SerializeField] private int roadGeneratorsComplete = 0;

    public override void Initialize() {
        gridManager = World.Instance.GetGridManager();
        float halfPos = (gridManager.GetGridTileSize() * World.Instance.GetGridManager().GetSize() * Chunk.size) / 2;
        transform.position = World.Instance.GetGridManager().transform.position + new Vector3(halfPos, 0, halfPos);
    }
    
    public override void Process() {
        if (gridManager.IsComplete()) {
            if (roadGenStage == EnumGenerationStage.STARTED && roadGeneratorInstances > 0) {
                if (roadGeneratorInstances == roadGeneratorsComplete) {
                    SetComplete();
                }
            }
            
            if (roadGenStage != EnumGenerationStage.STARTED) {
                BeginRoadGeneration();
                roadGenStage = EnumGenerationStage.STARTED;
            }
        }
    }
    
    private void BeginRoadGeneration() {
        Debug.Log("Generating start point for road generation");
        TilePos pos = TilePos.GetGridPosFromLocation(transform.position);
        IndividualPoint(EnumDirection.NORTH, pos);
        IndividualPoint(EnumDirection.EAST,  pos);
        IndividualPoint(EnumDirection.SOUTH, pos);
        IndividualPoint(EnumDirection.WEST,  pos);
        
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        Chunk chunk = gridManager.GetChunk(chunkPos);
        chunk.FillChunkCell(TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetId(), LocalPos.FromTilePos(pos), EnumDirection.NORTH, false);
    }

    private void IndividualPoint(EnumDirection dir, TilePos currentPos) {
        TilePos startPos = Direction.OffsetPos(dir, currentPos);
        GameObject genNorth = Instantiate(roadGenerator, TilePos.GetWorldPosFromTilePos(startPos), Quaternion.identity, transform);
        genNorth.name = dir + " InitGen";
        genNorth.GetComponent<RoadGenerator>().SetDirection(dir);
    }
    
    public void AddRoadGen() { roadGeneratorInstances++; }
    public void AddRoadGenComplete() => roadGeneratorsComplete++;
    
    /////////////////////////////////// Abstract inheritence stuff ///////////////////////////////////
    public override int GetGenerationPercentage() {
        if (roadGeneratorInstances > 0) {
            return (int) (((float) roadGeneratorsComplete / roadGeneratorInstances) * 100);
        }

        return 0;
    }

    public override string GetGenerationString() {
        return "Complete / Total road gen instances: " + roadGeneratorsComplete + "/" + roadGeneratorInstances;
    }
}