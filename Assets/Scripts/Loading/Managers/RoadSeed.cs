using Tiles.TileManagement;
using UnityEngine;

public class RoadSeed : GenerationSystem {
    
    private ChunkManager chunkManager;
    
    private EnumGenerationStage roadGenStage = EnumGenerationStage.INITIALIZED;

    [SerializeField] private GameObject roadGenerator = null;
    [ReadOnly, SerializeField] private int roadGeneratorInstances = 0;
    [ReadOnly, SerializeField] private int roadGeneratorsComplete = 0;

    public override void Initialize() {
        chunkManager = World.Instance.GetChunkManager();
        float halfPos = (chunkManager.GetGridTileSize() * chunkManager.GetSize() * Chunk.size) / 2;
        transform.position = chunkManager.transform.position + new Vector3(halfPos, 0, halfPos);
    }
    
    public override void Process() {
        if (chunkManager.IsComplete()) {
            if (roadGenStage == EnumGenerationStage.STARTED && roadGeneratorInstances > 0) {
                if (roadGeneratorInstances == roadGeneratorsComplete) {
                    ChunkManager chunkMan = World.Instance.GetChunkManager();
                    int maxPos = chunkMan.GetSize() * Chunk.size - 1;
                    
                    chunkMan.SetTile(new TilePos(0, 0), TileRegistry.ROAD_WORLD_EDGE_CORNER.GetId(), EnumDirection.WEST);
                    chunkMan.SetTile(new TilePos(0, maxPos), TileRegistry.ROAD_WORLD_EDGE_CORNER.GetId(), EnumDirection.NORTH);
                    chunkMan.SetTile(new TilePos(maxPos, maxPos), TileRegistry.ROAD_WORLD_EDGE_CORNER.GetId(), EnumDirection.EAST);
                    chunkMan.SetTile(new TilePos(maxPos, 0), TileRegistry.ROAD_WORLD_EDGE_CORNER.GetId(), EnumDirection.SOUTH);
                    
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
        TilePos pos = TilePos.GetTilePosFromLocation(transform.position);
        IndividualPoint(EnumDirection.NORTH, pos);
        IndividualPoint(EnumDirection.EAST,  pos);
        IndividualPoint(EnumDirection.SOUTH, pos);
        IndividualPoint(EnumDirection.WEST,  pos);
        
        ChunkPos chunkPos = TilePos.GetParentChunk(pos);
        Chunk chunk = chunkManager.GetChunk(chunkPos);
        chunk.FillChunkCell(TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetId(), LocalPos.FromTilePos(pos), EnumDirection.NORTH);
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