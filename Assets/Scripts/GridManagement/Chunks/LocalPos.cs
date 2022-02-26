[System.Serializable]
public class LocalPos : Position{
    public static LocalPos ZERO = new LocalPos(0, 0);

    public LocalPos(int x, int z) : base(x, z) {}

    public static LocalPos FromTilePos(TilePos tilePos) {
        ChunkPos chunkPos = TilePos.GetParentChunk(tilePos);

        return new LocalPos(chunkPos.ChunkTileX(tilePos), chunkPos.ChunkTileZ(tilePos));
    }

    public static LocalPos ValidatePos(LocalPos lp) {
        return new LocalPos(lp.x % Chunk.size, lp.z % Chunk.size);
    }

    public bool IsValidPos() {
        if (x < 0) return false;
        if (z < 0) return false;
        if (x > Chunk.size) return false;
        if (x > Chunk.size) return false;
        return true;
    }
}