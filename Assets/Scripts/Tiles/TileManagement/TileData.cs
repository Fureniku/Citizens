using System;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class TileData : MonoBehaviour {

    protected String tileName = "Air";
    protected int tileId = 0;
    [SerializeField] protected int width = 1;
    [SerializeField] protected int length = 1;
    
    [SerializeField] protected int gridX = 0;
    [SerializeField] protected int gridZ = 0;
    
    [SerializeField] protected bool halfRotations = false;

    [SerializeField] protected EnumTileDirection rotation = 0;
    [SerializeField] protected EnumTile enumTile;
    protected Tile tile;

    [SerializeField] protected EnumGenerateDirection genDirection = EnumGenerateDirection.NONE;

    public void Initialize() {
        tile = TileRegistry.GetTile(enumTile);
        tileName = tile.GetName();
        tileId = tile.GetId();
    }

    public void SetInitialPos() {
        TilePos tilePos = TilePos.GetGridPosFromLocation(transform.position);
        SetGridPos(tilePos);
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

    protected void SetId(int idIn) => tileId = idIn;
    protected void SetName(string nameIn) => name = nameIn;

    protected void SetRowCol(int rowIn, int colIn) {
        gridX = rowIn;
        gridZ = colIn;
    }

    public void SetRotation(EnumTileDirection rot) {
        rotation = rot;
        transform.rotation = Quaternion.Euler(transform.rotation.x, rot.GetRotation(),  transform.rotation.z);
    }
    
    public void SetGenerationDirection(EnumGenerateDirection dir) {
        genDirection = dir;
    }

    public void SetGridPos(TilePos pos) {
        gridX = pos.x;
        gridZ = pos.z;
    }

    public TilePos GetGridPos() {
        return new TilePos(gridX, gridZ);
    }

    public EnumTileDirection GetRotation() {
        return rotation;
    }

    public bool IsHalfRotation() {
        return halfRotations;
    }

    public bool RotationMatch(EnumTileDirection otherRot) {
        if (rotation == otherRot) {
            return true;
        }
        if (halfRotations) {
            return rotation == otherRot.Opposite();
        }
        return false;
    }

    //Just used for debugging
    public string GetTileDataForPrint() {
        return "Tile [" + GetId() + "] (" + GetName() + ") is at [" + gridX + ", " + gridZ + "]";
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
    
    public static int ParseInt(JToken token) {
        int result = 0;

        if (token != null) {
            try {
                result = Int32.Parse(token.ToString());
            }
            catch (FormatException) {
                Debug.Log("SaveLoadChunk failed to parse int from string: " + token);
            }
        }
        else {
            Debug.Log("SaveLoadChunk: Token was null!");
        }
        
        return result;
    }

    public abstract JProperty SerializeTile(int row, int col);

    public abstract void DeserializeTile(JObject json);
}
