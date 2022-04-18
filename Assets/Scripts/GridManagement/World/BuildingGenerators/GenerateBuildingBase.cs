﻿using Tiles.TileManagement;
using UnityEngine;

public abstract class GenerateBuildingBase {
    
    protected int width;
    protected int length;
    protected int height;

    private TilePos startPos;
    private GameObject buildingParent;

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
                TilePos placePos = new TilePos(startPos.x + w, startPos.z + l);
                EnumDirection rot = EnumDirection.NORTH;
                int id = SelectGameObject(w, l, ref rot);
                if (id > 0) {
                    World.Instance.GetChunkManager().SetTile(placePos, id, rot, buildingParent.transform);
                    World.Instance.GetChunkManager().GetTile(placePos).GetComponent<TileBuildingSegment>().MakeReady(height);
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

    protected abstract int SelectGameObject(int w, int l, ref EnumDirection rot);
}