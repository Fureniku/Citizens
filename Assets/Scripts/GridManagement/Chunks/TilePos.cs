using System;
using UnityEngine;

public class TilePos {

    public int x;
    public int z;
    
    public static TilePos ZERO = new TilePos(0, 0);

    public TilePos(int x, int z) {
        this.x = x;
        this.z = z;
    }
    
    public override string ToString() {
        return "[" + x + "," + z + "]";
    }

    //Convert a Unity coordinate location into a tilepos location
    public static TilePos GetGridPosFromLocation(Vector3 worldPos) {
        GridManager gm = GridManager.Instance;
        Vector3 gridStartPos = gm.transform.position;
        float tileSize = gm.GetGridTileSize();

        float xFinal = (worldPos.x - gridStartPos.x) / tileSize;
        float zFinal = (worldPos.z - gridStartPos.z) / tileSize;

        return new TilePos((int) Math.Floor(xFinal), (int) Math.Floor(zFinal));
    }

    public static Vector3 GetWorldPosFromTilePos(TilePos pos) {
        return new Vector3(pos.x * GridManager.Instance.GetGridTileSize(), 0, pos.z * GridManager.Instance.GetGridTileSize());
    }

    public static ChunkPos GetParentChunk(TilePos pos) {
        int xFinal = pos.x / Chunk.size;
        int zFinal = pos.z / Chunk.size;

        return new ChunkPos(xFinal, zFinal);
    }
}
