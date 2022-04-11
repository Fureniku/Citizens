using System;
using Loading.States;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateCarPark {
    
    private EnumTile entrance = EnumTile.CAR_PARK_ENTRANCE;
    private EnumTile edge = EnumTile.CAR_PARK_EDGE_BASE;
    private EnumTile corner = EnumTile.CAR_PARK_CORNER_BASE;
    private EnumTile inner = EnumTile.CAR_PARK_INNER_BASE;
    private EnumTile ramp = EnumTile.CAR_PARK_RAMP_BASE;

    private int width;
    private int length;
    private int height;

    private TilePos startPos;
    private GameObject buildingParent;

    public GenerateCarPark(TilePos startPos, int width, int minHeight, int maxHeight, int length) {
        Init(startPos, width, width, minHeight, maxHeight, length, length);
    }
    
    public GenerateCarPark(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) {
        Init(startPos, minWidth, maxWidth, minHeight, maxHeight, minLength, maxLength);
    }

    private void Init(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) {
        this.startPos = startPos;

        ChunkPos chunkpos = ChunkPos.GetChunkPosFromLocation(startPos.GetWorldPos());
        GameObject chunkParent = GameObject.Find($"chunk_{chunkpos.x}_{chunkpos.z}");
        
        this.width = Random.Range(minWidth, maxWidth);
        this.length = Random.Range(minLength, maxLength);
        this.height = Random.Range(minHeight, maxHeight);
        
        buildingParent = new GameObject("BuildingParent");
        buildingParent.transform.parent = chunkParent.transform;
        buildingParent.AddComponent<MeshCombiner>();
        buildingParent.AddComponent<MeshFilter>();
        buildingParent.AddComponent<MeshRenderer>();
        MeshCombinerManager.RegisterMeshCombiner(buildingParent);
    }

    public void Generate() {
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                TilePos placePos = new TilePos(startPos.x + w, startPos.z + l);
                EnumDirection rot = EnumDirection.NORTH;
                int id = SelectGameObject(w, l, ref rot);
                World.Instance.GetChunkManager().SetTile(placePos, id, rot, buildingParent.transform);
                World.Instance.GetChunkManager().GetTile(placePos).GetComponent<TileBuildingSegment>().MakeReady(height);
            }
        }
    }

    private int SelectGameObject(int l, int w, ref EnumDirection rot) {
        //ok this is gonna be fun
        //Check for entrance, at the front of the building
        if (l == 0) {
            //Arrays are zero-based, so minus 1 where appropriate.
            if (width % 2 == 0) { //Even-width building
                if (w == width / 2 || w == width / 2 - 1) {
                    return TileRegistry.GetTile(entrance).GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)Math.Floor(width / 2.0)) {
                    return TileRegistry.GetTile(entrance).GetId();
                }
            }
        }
        //Check for ramps, every level one row behind where the entrance is
        if (l == 1) {
            if (width % 2 == 0) { //Even-width building
                if (w == width / 2 || w == width / 2 - 1) {
                    return TileRegistry.GetTile(ramp).GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)Math.Floor(width / 2.0)) {
                    return TileRegistry.GetTile(ramp).GetId();
                }
            }
        }

        if (w == 0) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return TileRegistry.GetTile(corner).GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.SOUTH;
                return TileRegistry.GetTile(corner).GetId();
            }

            rot = EnumDirection.WEST;
            return TileRegistry.GetTile(edge).GetId();
        }
        if (w == width-1) {
            if (l == 0) {
                rot = EnumDirection.NORTH;
                return TileRegistry.GetTile(corner).GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return TileRegistry.GetTile(corner).GetId();
            }
            rot = EnumDirection.EAST;
            return TileRegistry.GetTile(edge).GetId();
        }

        if (l == 0) {
            return TileRegistry.GetTile(edge).GetId();
        }

        if (l == length - 1) {
            rot = EnumDirection.SOUTH;
            return TileRegistry.GetTile(edge).GetId();
        }
        
        return TileRegistry.GetTile(inner).GetId();
    }
}