using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : GenerationSystem {
    
    private List<Section> sections = new List<Section>();
    private int worldSize;

    private int progressZ;
    
    public override void Initialize() {
        worldSize = World.Instance.GetChunkManager().GetSize() * Chunk.size;
        StartCoroutine(Scan());
    }

    public IEnumerator Scan() {
        for (int col = 0; col < worldSize; col++) {
            for (int row = 0; row < worldSize; row++) {
                TilePos tilePos = new TilePos(row, col);
                TileData tileData = World.Instance.GetChunkManager().GetTile(tilePos);
                if (tileData.GetTile() == TileRegistry.GRASS) {
                    TileGrass grass = (TileGrass) tileData;

                    if (!grass.IsInSection()) {
                        sections.Add(NewSection(tilePos));
                    }
                }
            }
            progressZ = col;
            yield return null;
        }
        Debug.Log("section scanning complete. Created " + sections.Count + " sections.");
        SetComplete();
        yield return null;
    }
    
    Section NewSection(TilePos startPos) {
        Debug.Log("Starting new section at " + startPos);
        
        int maxX = worldSize - startPos.x;
        int maxZ = worldSize - startPos.z;
        int sizeX = 0;
        int sizeZ = 0;

        for (int row = 0; row < maxX; row++) {
            TilePos tilePos = new TilePos(startPos.x + row, startPos.z);
            if (ValidTileForSection(tilePos)) {
                sizeX++;
            }
            else {
                break;
            }
        }
        
        for (int col = 0; col < maxZ; col++) {
            TilePos tilePos = new TilePos(startPos.x, startPos.z + col);
            if (ValidTileForSection(tilePos)) {
                sizeZ++;
            }
            else {
                break;
            }
        }

        for (int col = 0; col < sizeZ; col++) {
            for (int row = 0; row < sizeX; row++) {
                TilePos tilePos = new TilePos(startPos.x + row, startPos.z + col);
                TileData tileData = World.Instance.GetChunkManager().GetTile(tilePos);
                if (tileData.GetTile() == TileRegistry.GRASS) {
                    TileGrass grass = (TileGrass) tileData;
                    grass.AddToSection();
                }
                else {
                    Debug.LogError("Tile " + tileData.name + " @ " + tilePos +  "was not grass; section is wrong!");
                }
            }
        }
        
        Debug.Log("Creating new section from " + startPos + " with size " + sizeX + ", " + sizeZ);
        return new Section(startPos, sizeX, sizeZ);
    }

    private bool ValidTileForSection(TilePos pos) {
        TileData tileData = World.Instance.GetChunkManager().GetTile(pos);
        
        if (tileData.GetTile() == TileRegistry.GRASS) {
            TileGrass grass = (TileGrass) tileData;
            if (!grass.IsInSection()) {
                return true;
            }
        }

        return false;
    }
    
    
    
    public override int GetGenerationPercentage() {
        return (int)(progressZ / (float)worldSize * 100);
    }

    public override string GetGenerationString() {
        return "Generating building sections";
    }

    public override void Process() {
        
    }
}