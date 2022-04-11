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

    public bool CanFit(GameObject go) {
        TileData data = go.GetComponent<TileData>();
        if (data != null) {
            return (data.GetWidth() <= sizeX && data.GetLength() <= sizeZ) || (data.GetWidth() <= sizeZ && data.GetLength() <= sizeX);
        }
        return false;
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