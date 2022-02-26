using System;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class TileData : MonoBehaviour {

    [ReadOnly, SerializeField] protected String tileName = "Air";
    [ReadOnly, SerializeField] protected int tileId = 0;
    
    [SerializeField] protected int width = 1;
    [SerializeField] protected int length = 1;
    
    //Position relative to the entire world
    [SerializeField] protected TilePos worldPos;
    //Position relative to the current chunk
    [SerializeField] protected LocalPos localPos;
    //The parent chunk itself
    [SerializeField] protected ChunkPos parentChunk;
    [SerializeField] protected bool halfRotations = false;

    [SerializeField] protected EnumTileDirection rotation = EnumTileDirection.NORTH;
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
        SetParentChunk(TilePos.GetParentChunk(tilePos));
        SetLocalPos(new LocalPos(parentChunk.ChunkTileX(tilePos), parentChunk.ChunkTileZ(tilePos)));
        transform.rotation = Quaternion.Euler(0, Direction.GetRotation(rotation), 0);
    }
    
    public void SetRotation(EnumTileDirection rot, bool debug = false) {
        if (debug) Debug.Log("setting rotation from " + rotation + " to  " + rot);
        rotation = rot;
        transform.rotation = Quaternion.Euler(transform.rotation.x, rot.GetRotation(),  transform.rotation.z);
    }

    public void SetGridPos(TilePos pos) => worldPos = pos;
    protected void SetLocalPos(LocalPos vec) => localPos = vec;
    protected void SetParentChunk(ChunkPos chunkPos) => parentChunk = chunkPos;

    public TilePos GetGridPos() { return worldPos; }
    public LocalPos GetLocalPos() { return localPos; }

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
        return "Tile [" + GetId() + "] (" + GetName() + ") is at [" + worldPos.x + ", " + worldPos.z + "]";
    }

    public static TileData GetFromGameObject(GameObject go) {
        if (go != null) {
            return go.GetComponent<TileData>();
        }

        return null;
    }

    private void OnDestroy() { //TODO just the chunk, not the whole grid.
        World.Instance.GetGridManager().FlagForRecheck();
    }
    
    
    public int GetId() { return tileId; }
    public String GetName() { return tileName; }
    public int GetWidth() { return width; }
    public int GetLength() { return length; }
    protected void SetId(int idIn) => tileId = idIn;
    protected void SetName(string nameIn) => name = nameIn;
    public void SetGenerationDirection(EnumGenerateDirection dir) { genDirection = dir; }
    public EnumTileDirection GetRotation() { return rotation; }

    //////////////// Used for load/save
    
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

    public abstract JProperty SerializeTile(TileData td, int row, int col);

    public abstract void DeserializeTile(JObject json);
}
