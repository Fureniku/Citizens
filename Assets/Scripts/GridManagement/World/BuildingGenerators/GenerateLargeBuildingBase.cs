using Tiles.TileManagement;
using UnityEngine;

public abstract class GenerateLargeBuildingBase : GenerateBuildingBase {

    private int buildingId;
    private EnumDirection rotation;
    protected TileData building;

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
        } else if (w == startW + building.GetWidth() && (l == startL + building.GetLength() || l == startL - 1)) {
            if (data.GetTile() == TileRegistry.GRASS) {
                chunkManager.SetTile(pos, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.NORTH);
            }
        } else {
            // Side roads!
            if (w == startW - 1 || w == startW + building.GetWidth()) {
                if (data.GetTile() == TileRegistry.GRASS) {
                    chunkManager.SetTile(pos, TileRegistry.STRAIGHT_ROAD_1x1.GetId(), EnumDirection.NORTH);
                }

                if (l == 0) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.NORTH, pos);
                    Tile tile = chunkManager.GetTile(offset).GetTile();
                    if (tile.GetTileType() == TileType.ROAD) {
                        if (tile == TileRegistry.T_JUNCT_ROAD_1x1) {
                            chunkManager.SetTile(offset, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.WEST);
                        } else {
                            chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.WEST);
                        }
                    }
                }
                
                if (l == length-1) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.SOUTH, pos);
                    Tile tile = chunkManager.GetTile(offset).GetTile();
                    if (tile.GetTileType() == TileType.ROAD) {
                        if (tile == TileRegistry.T_JUNCT_ROAD_1x1) {
                            chunkManager.SetTile(offset, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.EAST);
                        } else {
                            chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.EAST);
                        }
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
                    Tile tile = chunkManager.GetTile(offset).GetTile();
                    if (tile.GetTileType() == TileType.ROAD) {
                        if (tile == TileRegistry.T_JUNCT_ROAD_1x1) {
                            chunkManager.SetTile(offset, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.NORTH);
                        } else {
                            chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.NORTH);
                        }
                    }
                }

                if (w == width - 1) {
                    TilePos offset = Direction.OffsetPos(EnumDirection.WEST, pos);
                    Tile tile = chunkManager.GetTile(offset).GetTile();
                    if (tile.GetTileType() == TileType.ROAD) {
                        if (tile == TileRegistry.T_JUNCT_ROAD_1x1) {
                            chunkManager.SetTile(offset, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.SOUTH);
                        } else {
                            chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.SOUTH);
                        }
                    }
                }
            }
        }
        
        return -1;
    }


    public GenerateLargeBuildingBase(Section section, int buildingId, EnumDirection rotation, TileData building)
        : base(section.GetTilePos(), section.GetSizeX(), 1, 1, section.GetSizeZ()) {
        this.buildingId = buildingId;
        this.rotation = rotation;
        this.building = building;
    }
}