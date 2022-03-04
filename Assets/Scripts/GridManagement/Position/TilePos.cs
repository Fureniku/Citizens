using System;
using UnityEngine;

[System.Serializable]
public class TilePos : Position {

    public static TilePos ZERO = new TilePos(0, 0);

    public TilePos(int x, int z) : base(x, z){}

    //Convert a Unity coordinate location into a tilepos location
    public static TilePos GetGridPosFromLocation(Vector3 worldPos) {
        Vector3 gridStartPos = World.Instance.GetGridManager().transform.position;
        float tileSize = World.Instance.GetGridManager().GetGridTileSize();

        float xFinal = (worldPos.x - gridStartPos.x) / tileSize;
        float zFinal = (worldPos.z - gridStartPos.z) / tileSize;

        return new TilePos((int) Math.Floor(xFinal), (int) Math.Floor(zFinal));
    }

    public static Vector3 GetWorldPosFromTilePos(TilePos pos) {
        return new Vector3(pos.x * World.Instance.GetGridManager().GetGridTileSize(), 0, pos.z * World.Instance.GetGridManager().GetGridTileSize());
    }

    public Vector3 GetWorldPos() {
        return GetWorldPosFromTilePos(this);
    }

    public static ChunkPos GetParentChunk(TilePos pos) {
        int xFinal = pos.x / Chunk.size;
        int zFinal = pos.z / Chunk.size;

        return new ChunkPos(xFinal, zFinal);
    }
}
