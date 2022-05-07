using System;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public abstract class TileData : MonoBehaviour {

    [SerializeField] protected String tileName = "Air";
    [SerializeField] protected int tileId = 0;
    
    [SerializeField] protected int width = 1;
    [SerializeField] protected int length = 1;
    
    //Position relative to the entire world
    [SerializeField] protected TilePos worldPos;
    //Position relative to the current chunk
    [SerializeField] protected LocalPos localPos;
    //The parent chunk itself
    [SerializeField] protected ChunkPos parentChunk;
    [SerializeField] protected bool halfRotations = false;

    [SerializeField] protected EnumDirection rotation = EnumDirection.NORTH;


    [SerializeField] protected EnumGenerateDirection genDirection = EnumGenerateDirection.NONE;
    
    protected bool isRegistryEntry = true; //Whether this copy of the object is for registration, preventing procedural generations. Set to false when legitimately spawning.

    public void Initialize() {
        tileName = TileRegistry.GetTile(tileId).GetName();
    }

    public void SetInitialPos() {
        TilePos tilePos = TilePos.GetTilePosFromLocation(transform.position);
        SetGridPos(tilePos);
        SetParentChunk(TilePos.GetParentChunk(tilePos));
        SetLocalPos(new LocalPos(parentChunk.ChunkTileX(tilePos), parentChunk.ChunkTileZ(tilePos)));
        transform.rotation = Quaternion.Euler(0, Direction.GetRotation(rotation), 0);
        
        UpdateTile();
    }
    
    public void SetRotation(EnumDirection rot, bool debug = false) {
        if (debug) Debug.Log("setting rotation from " + rotation + " to  " + rot);
        rotation = rot;
        transform.rotation = Quaternion.Euler(transform.rotation.x, rot.GetRotation(),  transform.rotation.z);
    }

    public void SetGridPos(TilePos pos) => worldPos = pos;
    protected void SetLocalPos(LocalPos vec) => localPos = vec;
    protected void SetParentChunk(ChunkPos chunkPos) => parentChunk = chunkPos;

    public TilePos GetTilePos() { return worldPos; }
    public LocalPos GetLocalPos() { return localPos; }

    public bool IsHalfRotation() {
        return halfRotations;
    }

    public bool RotationMatch(EnumDirection otherRot) {
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
    
    public int GetId() { return tileId; }
    public String GetName() { return tileName; }
    public int GetWidth() { return width; }
    public int GetLength() { return length; }
    protected void SetId(int idIn) => tileId = idIn;
    protected void SetName(string nameIn) => name = nameIn;
    public void SetGenerationDirection(EnumGenerateDirection dir) { genDirection = dir; }
    public EnumDirection GetRotation() { return rotation; }

    public Tile GetTile() {
        return TileRegistry.GetTile(tileId);
    }

    public bool IsRegistryVersion() {
        return transform.root.GetComponent<TileRegistry>() != null;
    }

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

    public void HideAfterRegistrationBase() {
        if (GetComponent<MeshRenderer>() != null) {
            GetComponent<MeshRenderer>().enabled = false;
        }
        isRegistryEntry = true;
    }

    public void CreateBase() {
        isRegistryEntry = false;
        if (GetComponent<MeshRenderer>() != null) {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void OnChanged() {
        ChunkManager chunkMan = World.Instance.GetChunkManager();
        TileData north = chunkMan.GetTile(worldPos.Offset(EnumDirection.NORTH));
        TileData east = chunkMan.GetTile(worldPos.Offset(EnumDirection.EAST));
        TileData south = chunkMan.GetTile(worldPos.Offset(EnumDirection.SOUTH));
        TileData west = chunkMan.GetTile(worldPos.Offset(EnumDirection.WEST));
        
        north.OnNeighbourChanged(EnumDirection.SOUTH);
        east.OnNeighbourChanged(EnumDirection.WEST);
        south.OnNeighbourChanged(EnumDirection.NORTH);
        west.OnNeighbourChanged(EnumDirection.EAST);
    }

    public abstract void UpdateTile(); //Called when the tile should be checked for updates, NOT frame based!!
    public abstract void OnNeighbourChanged(EnumDirection neighbour);
    
    public abstract JProperty SerializeTile(TileData td, int row, int col);
    public abstract void DeserializeTile(JObject json);
    public abstract void HideAfterRegistration();
    public abstract void CreateFromRegistry();
}
