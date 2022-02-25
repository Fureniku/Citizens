using System;
using UnityEngine;

public class ChunkPos {
    public int x;
    public int z;

    public static ChunkPos ZERO = new ChunkPos(0, 0);

    public ChunkPos(int x, int z) {
        this.x = x;
        this.z = z;
    }
    
    public override string ToString() {
        return "[" + x + "," + z + "]";
    }

    //Convert a Unity coordinate location into a tilepos location
    public static ChunkPos GetChunkPosFromLocation(Vector3 worldPos) {
        GridManager gm = GridManager.Instance;
        Vector3 gridStartPos = gm.transform.position;
        float tileSize = gm.GetGridTileSize();

        float xFinal = ((worldPos.x - gridStartPos.x) / tileSize) / Chunk.size;
        float zFinal = ((worldPos.z - gridStartPos.z) / tileSize) / Chunk.size;

        return new ChunkPos((int) Math.Floor(xFinal), (int) Math.Floor(zFinal));
    }

    public static Vector3 GetChunkOrigin(ChunkPos pos) {
        return new Vector3(pos.x * GridManager.Instance.GetGridTileSize(), 0, pos.z * GridManager.Instance.GetGridTileSize());
    }
}