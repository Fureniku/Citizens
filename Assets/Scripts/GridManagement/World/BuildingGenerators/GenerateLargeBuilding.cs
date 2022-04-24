﻿using Tiles.TileManagement;
using UnityEngine;

public class GenerateLargeBuilding : GenerateBuildingBase {

    private int buildingId;
    private EnumDirection rotation;
    private TileData building;

    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        int startW = 0;
        int startL = 0;

        if (width >= building.GetWidth() + 4) {
            startW = width / 2 - building.GetWidth() / 2;
        }

        if (length >= building.GetLength() + 4) {
            startL = length / 2 - building.GetLength() / 2;
        }

        if (w == startW && l == startL) {
            rot = rotation;
            return buildingId;
        }

        TileData td = TileRegistry.GetTileFromID(buildingId);

        if (w >= startW && w < td.GetWidth()+startW && l >= startL && l < td.GetLength()+startL) {
            return TileRegistry.REFERENCE.GetId();
        }
        
        ChunkManager chunkManager = World.Instance.GetChunkManager();
        TilePos pos = GetPosInSection(w, l);
        TileData data = chunkManager.GetTile(pos);
        
        
        // Crossings!
        if (w == startW - 1 && (l == startL + building.GetLength() || l == startL -1)) {
            if (data.GetTile() == TileRegistry.GRASS) {
                chunkManager.SetTile(pos, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.NORTH);
            }
        } else if (w == startW + building.GetWidth() && (l == startL + building.GetLength() || l == startL -1)) {
            if (data.GetTile() == TileRegistry.GRASS) {
                chunkManager.SetTile(pos, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.NORTH);
            }
        }
        else {
            // Side roads!
            if (w == startW - 1 || w == startW + building.GetWidth()) {
                if (data.GetTile() == TileRegistry.GRASS) {
                    chunkManager.SetTile(pos, TileRegistry.STRAIGHT_ROAD_1x1.GetId(), EnumDirection.NORTH);
                }

                if (l == 0) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.NORTH, pos);
                    if (chunkManager.GetTile(offset).GetTile().GetTileType() == TileType.ROAD) {
                        chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.WEST);
                    }
                }
                
                if (l == length-1) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.SOUTH, pos);
                    if (chunkManager.GetTile(offset).GetTile().GetTileType() == TileType.ROAD) {
                        chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.EAST);
                    }
                }
            }

            if (l == startL - 1 || l == startL + building.GetLength()) {
                //bottom road
                if (data.GetTile() == TileRegistry.GRASS) {
                    chunkManager.SetTile(pos, TileRegistry.STRAIGHT_ROAD_1x1.GetId(), EnumDirection.EAST);
                }

                if (w == 0) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.EAST, pos);
                    if (chunkManager.GetTile(offset).GetTile().GetTileType() == TileType.ROAD) {
                        chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.NORTH);
                    }
                }

                if (w == width - 1) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.WEST, pos);
                    Debug.LogError("testing at position " + offset);
                    if (chunkManager.GetTile(offset).GetTile().GetTileType() == TileType.ROAD) {
                        chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.SOUTH);
                    }
                }
            }
        }


        return -1;
    }

    public GenerateLargeBuilding(TilePos startPos, int width, int length, int buildingId, EnumDirection rotation, TileData building)
        : base(startPos, width, 1, 1, length) {
        this.buildingId = buildingId;
        this.rotation = rotation;
        this.building = building;
    }
}