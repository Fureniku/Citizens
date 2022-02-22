using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class TileData : MonoBehaviour {

    [SerializeField] protected String tileName = "Air";
    [SerializeField] protected int tileId = 0;
    [SerializeField] protected int width = 1;
    [SerializeField] protected int length = 1;
    
    [SerializeField] protected int gridX = 0;
    [SerializeField] protected int gridZ = 0;
    
    [SerializeField] protected bool halfRotations = false;

    [SerializeField] protected int rotation = 0;

    [SerializeField] protected int randomUUID = 0;
    
    [SerializeField] protected EnumGenerateDirection genDirection = EnumGenerateDirection.NONE;

    void Start() {
        randomUUID = Random.Range(1, 10000);
        TilePos tilePos = TilePos.GetGridPosFromLocation(transform.position);
        SetGridPos(tilePos);
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

    public int GetWidth() {
        return width;
    }

    public int GetLength() {
        return length;
    }

    public void SetRotation(int rot) {
        rotation = rot;
        transform.rotation = Quaternion.Euler(transform.rotation.x, rot,  transform.rotation.z);
    }
    
    public void SetGenerationDirection(EnumGenerateDirection dir) {
        genDirection = dir;
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

    private void OnDestroy() {
        GridManager.Instance.FlagForRecheck();
    }
}
