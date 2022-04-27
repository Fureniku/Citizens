using Tiles.TileManagement;
using UnityEngine;

public abstract class GenerateBuildingBase {
    
    protected int width;
    protected int length;
    protected int height;

    protected TilePos startPos;
    private GameObject buildingParent;

    protected bool generate;

    public GenerateBuildingBase(TilePos startPos, int width, int minHeight, int maxHeight, int length) {
        Init(startPos, width, width, minHeight, maxHeight, length, length);
    }
    
    public GenerateBuildingBase(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) {
        Init(startPos, minWidth, maxWidth, minHeight, maxHeight, minLength, maxLength);
    }

    private void Init(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) {
        this.startPos = startPos;

        ChunkPos chunkpos = ChunkPos.GetChunkPosFromLocation(startPos.GetWorldPos());
        GameObject chunkParent = GameObject.Find($"chunk_{chunkpos.x}_{chunkpos.z}");
        
        this.width = Random.Range(minWidth, maxWidth);
        this.length = Random.Range(minLength, maxLength);
        this.height = Random.Range(minHeight, maxHeight);
        
        buildingParent = new GameObject($"BuildingParent_{startPos.x}_{startPos.z}");
        buildingParent.transform.parent = chunkParent.transform;
        buildingParent.AddComponent<MeshCombiner>();
        buildingParent.AddComponent<MeshFilter>();
        buildingParent.AddComponent<MeshRenderer>();
    }
    
    public void Generate() {
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                TilePos placePos = GetPosInSection(w, l);
                EnumDirection rot = EnumDirection.NORTH;
                int id = SelectGameObject(w, l, ref rot);
                if (id > 0) {
                    World.Instance.GetChunkManager().SetTile(placePos, id, rot, buildingParent.transform);
                    TileData tile = World.Instance.GetChunkManager().GetTile(placePos);
                    if (tile.GetComponent<TileBuildingSegment>() != null) {
                        tile.GetComponent<TileBuildingSegment>().MakeReady(height);
                    }
                }
            }
        }
    }

    public TilePos GetPosInSection(int w, int l) {
        return new TilePos(startPos.x + w, startPos.z + l);
    }

    public void SetReferenceTiles() {
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                TilePos placePos = new TilePos(startPos.x + w, startPos.z + l);
                TileData td = World.Instance.GetChunkManager().GetTile(placePos);
                if (td is TileReference) {
                    TileReference referenceTile = (TileReference) td;
                    
                    referenceTile.SetMasterTile(startPos);
                }
            }
        }
    }

    public void CombineMeshes() {
        TilePos placePos = new TilePos(startPos.x, startPos.z);
        GameObject go = World.Instance.GetChunkManager().GetTile(placePos).gameObject;
        Debug.Log("Getting parent for building: " + go.transform.parent.name);
        if (go.transform.parent.GetComponent<MeshCombiner>() != null) {
            go.transform.parent.GetComponent<MeshCombiner>().CombineMeshes();
        }
    }
    
    protected int PlaceSimpleObject(int w, int l, ref EnumDirection rot, BuildingTypeRegister btr) {
        if (width <= 2) {
            if (w == 0) {
                rot = EnumDirection.NORTH;
            } else {
                rot = EnumDirection.SOUTH;
            }
            return btr.GetNextBuilding().GetId();
        } else if (length <= 2) {
            if (l == 0) {
                rot = EnumDirection.WEST;
            } else {
                rot = EnumDirection.EAST;
            }
            return btr.GetNextBuilding().GetId();
        } else {
            if (w == 0) {
                rot = EnumDirection.NORTH;
                return btr.GetNextBuilding().GetId();
            }
            if (w == width-1) {
                rot = EnumDirection.SOUTH;
                return btr.GetNextBuilding().GetId();
            }

            if (l == 0) {
                rot = EnumDirection.WEST;
                return btr.GetNextBuilding().GetId();
            }

            if (l == length - 1) {
                rot = EnumDirection.EAST;
                return btr.GetNextBuilding().GetId();
            }
        }
        
        return -1;
    }
    
    protected void SetJunctionIn(TilePos placePos, EnumDirection dir) {
        ChunkManager chunkManager = World.Instance.GetChunkManager();
        TileData tile = chunkManager.GetTile(placePos);
        
        if (tile.GetId() == TileRegistry.CROSSROAD_ROAD_1x1.GetId() || tile.GetId() == TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetId()) {
            chunkManager.SetTile(placePos, tile.GetId(), dir);
        } else if (tile.GetId() == TileRegistry.T_JUNCT_ROAD_1x1.GetId()) {
            chunkManager.SetTile(placePos, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), dir);
        } else {
            chunkManager.SetTile(placePos, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), dir);
        }
    }

    protected abstract int SelectGameObject(int w, int l, ref EnumDirection rot);
    public abstract void PostGenerate();
}