using System.Collections;
using System.Collections.Generic;
using Loading.States;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;

public class TileRoad : TileData {

    [SerializeField] private bool streetLights = false;
    [SerializeField] private bool edge_north = false;
    [SerializeField] private bool edge_east = false;
    [SerializeField] private bool edge_south = false;
    [SerializeField] private bool edge_west = false;

    [SerializeField] private GameObject edge_wall;
    [SerializeField] private GameObject edge_pillar;

    private GameObject spawnedEdgeNorth = null;
    private GameObject spawnedEdgeEast = null;
    private GameObject spawnedEdgeSouth = null;
    private GameObject spawnedEdgeWest = null;
    
    private GameObject spawnedEdgeNorthEast = null;
    private GameObject spawnedEdgeNorthWest = null;
    private GameObject spawnedEdgeSouthEast = null;
    private GameObject spawnedEdgeSouthWest = null;

    void Start() {
        Initialize();
        width = 1;
        length = 1;
    }

    public override void UpdateTile() {
        Vector3 pos = transform.position;
        
        if (edge_wall != null) {
            edge_north = IsEdge(EnumDirection.NORTH);
            edge_east = IsEdge(EnumDirection.EAST);
            edge_south = IsEdge(EnumDirection.SOUTH);
            edge_west = IsEdge(EnumDirection.WEST);

            spawnedEdgeNorth = SetWall(edge_north, spawnedEdgeNorth, pos, 0);
            spawnedEdgeEast  = SetWall(edge_east,  spawnedEdgeEast,  pos, 90);
            spawnedEdgeSouth = SetWall(edge_south, spawnedEdgeSouth, pos, 180);
            spawnedEdgeWest  = SetWall(edge_west,  spawnedEdgeWest,  pos, 270);
        }

        if (edge_pillar != null) {
            spawnedEdgeNorthEast = SetCorner(edge_north && edge_east, spawnedEdgeNorthEast, pos, 0);
            spawnedEdgeNorthWest = SetCorner(edge_north && edge_west, spawnedEdgeNorthWest, pos, 270);
            spawnedEdgeSouthEast = SetCorner(edge_south && edge_east, spawnedEdgeSouthEast, pos, 90);
            spawnedEdgeSouthWest = SetCorner(edge_south && edge_west, spawnedEdgeSouthWest, pos, 180);
        }
    }

    private GameObject SetWall(bool exists, GameObject go, Vector3 pos, int rotation) {
        if (exists) {
            if (go == null) {
                go = Instantiate(edge_wall, pos, Quaternion.Euler(0, rotation, 0), transform);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
            
            return go;
        }
        if (go == null) Destroy(go);
        return null;
    }

    private GameObject SetCorner(bool exists, GameObject go, Vector3 pos, int rotation) {
        if (exists) {
            if (go == null) {
                go = Instantiate(edge_pillar, pos, Quaternion.Euler(0, rotation, 0), transform);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
            
            return go;
        }
        if (go == null) Destroy(go);
        return null;
    }

    private bool IsEdge(EnumDirection side) {
        TilePos pos = worldPos.Offset(side);
        if (pos.IsValid()) {
            TileData data = World.Instance.GetChunkManager().GetTile(pos);
            return data.GetTile().GetTileType() == TileType.AIR || data.GetTile().GetTileType() == TileType.LOWERED;
        }
        return true;
    }

    public override void OnNeighbourChanged(EnumDirection neighbour) {
        
    }
    
    
    public override JProperty SerializeTile(TileData data, int row, int col) {
        JObject jObj = new JObject();

        jObj.Add(new JProperty("id", data.GetId()));
        jObj.Add(new JProperty("rotation", data.GetRotation().GetRotation()));
        jObj.Add(new JProperty("row", data.GetTilePos().x));
        jObj.Add(new JProperty("col", data.GetTilePos().z));
        
        return new JProperty($"tile_{row}_{col}", jObj);
    }

    public override void DeserializeTile(JObject json) {
        SetId(ParseInt(json.GetValue("id")));
        SetName(tileName);
        SetRotation(Direction.GetDirection(ParseInt(json.GetValue("rotation"))));
        SetLocalPos(new LocalPos(ParseInt(json.GetValue("row")), ParseInt(json.GetValue("col"))));
        
        SetInitialPos();
    }
    
    public override void HideAfterRegistration() {
        HideAfterRegistrationBase();
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<MeshRenderer>() != null) {
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public override void CreateFromRegistry() {
        CreateBase();
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<MeshRenderer>() != null) {
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}