using System.Collections.Generic;
using UnityEngine;

public class SectionManager : GenerationSystem {
    
    private List<Section> sections = new List<Section>();

    public void Scan() {
        int worldSize = World.Instance.GetGridManager().GetSize();

        for (int row = 0; row < worldSize; row++) {
            for (int col = 0; col < worldSize; col++) {
                TilePos tilePos = new TilePos(row, col);
                TileData tileData = World.Instance.GetGridManager().GetTile(tilePos);
                if (tileData.GetTile() == TileRegistry.GRASS) {
                    TileGrass grass = (TileGrass) tileData;

                    if (!grass.IsInSection()) {
                        sections.Add(NewSection(tilePos));
                    }
                }
            }
        }
        Debug.Log("section scanning complete. Created " + sections.Count + " sections.");
    }
    
    Section NewSection(TilePos startPos) {
        int maxX = World.Instance.GetGridManager().GetSize() - startPos.x;
        int maxZ = World.Instance.GetGridManager().GetSize() - startPos.z;
        int sizeX = 0;
        int sizeZ = 0;

        for (int row = 0; row < maxX; row++) {
            TilePos tilePos = new TilePos(row, startPos.z);
            TileData tileData = World.Instance.GetGridManager().GetTile(tilePos);
            if (tileData.GetTile() == TileRegistry.GRASS) {
                TileGrass grass = (TileGrass) tileData;
                if (sizeX < row) sizeX = row;
            }
            else {
                Debug.Log("new section X: " + sizeX);
                break;
            }
        }
        
        for (int col = 0; col < maxZ; col++) {
            TilePos tilePos = new TilePos(startPos.x, col);
            TileData tileData = World.Instance.GetGridManager().GetTile(tilePos);
            if (tileData.GetTile() == TileRegistry.GRASS) {
                TileGrass grass = (TileGrass) tileData;
                if (sizeZ < col) sizeZ = col;
                grass.AddToSection();
            }
            else {
                Debug.Log("new section Z: " + sizeZ);
                break;
            }
        }
        Debug.Log("Creating new section from " + startPos + " with size " + sizeX + ", " + sizeZ);
        return new Section(startPos, sizeX, sizeZ);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    public override int GetGenerationPercentage() {
        throw new System.NotImplementedException();
    }

    public override string GetGenerationString() {
        throw new System.NotImplementedException();
    }
}