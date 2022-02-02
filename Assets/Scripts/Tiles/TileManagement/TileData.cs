using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileData : MonoBehaviour {

    [SerializeField] private String tileName = "Air";
    [SerializeField] private int tileId = 0;
    [SerializeField] private int width = 1;
    [SerializeField] private int length = 1;
    
    [SerializeField] private int gridX = 0;
    [SerializeField] private int gridZ = 0;
    
    [SerializeField] private bool halfRotations = false;

    public static TileData tileAir = new TileData();

    [SerializeField] private int rotation = 0;

    [SerializeField] private int randomUUID = 0;
    

    void Start() {
        randomUUID = Random.Range(1, 10000);
    }

    public int GetUUID() {
        return randomUUID;
    }

    public int GetId() {
        return tileId;
    }

    public String GetName() {
        return tileName;
    }

    public void SetRotation(int rot) {
        rotation = rot;
        transform.rotation = Quaternion.Euler(transform.rotation.x, rot,  transform.rotation.z);
    }

    public void SetGridPos(TilePos pos) {
        gridX = pos.x;
        gridZ = pos.z;
    }

    public int GetRotation() {
        return rotation;
    }

    public bool IsHalfRotation() {
        return halfRotations;
    }

    public bool RotationMatch(int otherRot) {
        if (rotation == otherRot) {
            return true;
        }
        if (halfRotations) {
            return Math.Abs(rotation - otherRot) == 180;
        }
        return false;
    }

    //Just used for debugging
    public string GetTileDataForPrint() {
        return "Tile [" + GetId() + "] (" + GetName() + ") is at [" + gridX + ", " + gridZ + "] with UID " + randomUUID;
    }

    public static TileData GetFromGameObject(GameObject go) {
        if (go != null) {
            return go.GetComponent<TileData>();
        }

        return null;
    }
}
