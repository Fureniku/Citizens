using System;
using UnityEngine;

[System.Serializable]
public class ChunkPos : Position{

    public static ChunkPos ZERO = new ChunkPos(0, 0);

    public ChunkPos(int x, int z) : base(x,z){}

    //Convert a Unity coordinate location into a tilepos location
    public static ChunkPos GetChunkPosFromLocation(Vector3 worldPos) {
        Vector3 gridStartPos = World.Instance.GetGridManager().transform.position;
        float tileSize = World.Instance.GetGridManager().GetGridTileSize();

        float xFinal = ((worldPos.x - gridStartPos.x) / tileSize) / Chunk.size;
        float zFinal = ((worldPos.z - gridStartPos.z) / tileSize) / Chunk.size;

        return new ChunkPos((int) Math.Floor(xFinal), (int) Math.Floor(zFinal));
    }

    public static Vector3 GetChunkOrigin(ChunkPos pos) {
        return new Vector3(pos.x * World.Instance.GetGridManager().GetGridTileSize(), 0, pos.z * World.Instance.GetGridManager().GetGridTileSize());
    }

    public int ChunkTileX(TilePos pos) {
        return pos.x - x * Chunk.size;
    }
    
    public int ChunkTileZ(TilePos pos) {
        return pos.z - z * Chunk.size;
    }
}