//A class which holds info on a detected section of grass

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section {
    private int sizeX;
    private int sizeZ;
    private TilePos startPos;

    public Section(TilePos startPos, int sizeX, int sizeZ) {
        this.startPos = startPos;
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
    }

    public bool CanFit(TileData data) {
        if (data != null) {
            return CanFit(data.GetWidth(),data.GetWidth());
        }
        return false;
    }

    public bool CanFit(int width, int length) {
        return width <= sizeX && length <= sizeZ || width <= sizeZ && length <= sizeX;
    }

    public void DeleteSection() {
        for (int row = 0; row < sizeX; row++) {
            for (int col = 0; col < sizeZ; col++) {
                TilePos pos = new TilePos(startPos.x + row, startPos.z + col);
                TileData data = World.Instance.GetChunkManager().GetTile(pos);

                if (data is TileGrass) {
                    TileGrass grass = (TileGrass) data;
                    grass.RemoveFromSection();
                }
            }
        }
    }

    public void Rescan(ref List<Section> sections) {
        int minX = sizeX;
        int minZ = sizeZ;
        for (int row = 0; row < sizeX; row++) {
            for (int col = 0; col < sizeZ; col++) {
                TilePos checkPos = new TilePos(startPos.x + row, startPos.z + col);
            }
        }
    }
    
    public TilePos GetTilePos() { return startPos; }
    public int GetSizeX() { return sizeX; }
    public int GetSizeZ() { return sizeZ; }
}