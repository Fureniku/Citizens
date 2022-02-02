using System;
using UnityEngine;

public class TilePos {

    public int x;
    public int z;

    public TilePos(int x, int z) {
        this.x = x;
        this.z = z;
    }

    public TilePos(Vector3 vec) {
        this.x = (int) Math.Floor(vec.x / 10);
        this.z = (int) Math.Floor(vec.z / 10);
    }

    public Vector3 GetVector3() {
        return new Vector3(x * 10 + 5, 0, z * 10 + 5);
    }

    public override string ToString() {
        return "[" + x + "," + z + "]";
    }

    //Create a tilepos from tile grid coordinates which must be positive
    public static TilePos MapOffset(int x, int z) {
        return new TilePos(x - TileGrid.GetMapSize() / 2, z - TileGrid.GetMapSize() / 2);
    }
}
