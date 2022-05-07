using System;
using Tiles.TileManagement;
using UnityEngine;

[System.Serializable]
public class TilePos : Position {

    public static TilePos ZERO = new TilePos(0, 0);

    public TilePos(int x, int z) : base(x, z){}

    //Convert a Unity coordinate location into a tilepos location
    public static TilePos GetTilePosFromLocation(Vector3 worldPos) {
        Vector3 gridStartPos = World.Instance.GetChunkManager().transform.position;
        float tileSize = World.Instance.GetChunkManager().GetGridTileSize();

        float xFinal = (worldPos.x - gridStartPos.x) / tileSize;
        float zFinal = (worldPos.z - gridStartPos.z) / tileSize;

        return new TilePos((int) Math.Floor(xFinal), (int) Math.Floor(zFinal));
    }

    public static Vector3 GetWorldPosFromTilePos(TilePos pos) {
        return new Vector3(pos.x * World.Instance.GetChunkManager().GetGridTileSize(), 0, pos.z * World.Instance.GetChunkManager().GetGridTileSize());
    }

    public Vector3 GetWorldPos() {
        return GetWorldPosFromTilePos(this);
    }

    public static ChunkPos GetParentChunk(TilePos pos) {
        int xFinal = pos.x / Chunk.size;
        int zFinal = pos.z / Chunk.size;

        return new ChunkPos(xFinal, zFinal);
    }

    public static int TileDistance(TilePos posA, TilePos posB) {
        int x = Math.Abs(posA.x - posB.x);
        int z = Math.Abs(posA.z - posB.z);
        
        return x+z;
    }

    public static bool IsValid(TilePos tpIn) {
        int size = World.Instance.GetChunkManager().GetSize() * Chunk.size;
        if (tpIn.x < 0 || tpIn.z < 0 || tpIn.x > size-1 || tpIn.z > size-1) return false;
        return true;
    }

    public static TilePos Clamp(TilePos tpIn) {
        int x = tpIn.x;
        int z = tpIn.z;
        int size = World.Instance.GetChunkManager().GetSize() * Chunk.size;

        if (x < 0) x = 0;
        if (z < 0) z = 0;
        if (x > size) x = size;
        if (z > size) z = size;

        return new TilePos(x, z);
    }

    public TilePos Offset(EnumDirection direction) {
        return direction.OffsetPos(this);
    }
    
    public TilePos Offset(EnumDirection direction, int count) {
        return direction.OffsetPos(this, count);
    }
    
    public bool IsValid() {
        int size = World.Instance.GetChunkManager().GetSize() * Chunk.size;
        if (x < 0 || z < 0 || x > size-1 || z > size-1) return false;
        return true;
    }
}
