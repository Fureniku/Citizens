using Tiles.TileManagement;
using UnityEngine;

public class GenerateHospital : GenerateBuildingBase {

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
                    if (chunkManager.GetTile(offset).GetTile().GetTileType() == TileType.ROAD) {
                        chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), EnumDirection.SOUTH);
                    }
                }
            }
        }
        
        return -1;
    }

    public override void PostGenerate() {
        int startW = 0;
        int startL = 0;

        if (width  >= building.GetWidth()  + 4) startW = width  / 2 - building.GetWidth()  / 2;
        if (length >= building.GetLength() + 4) startL = length / 2 - building.GetLength() / 2;

        ChunkManager chunkManager = World.Instance.GetChunkManager();

        TilePos placePos = new TilePos(startPos.x + startW, startPos.z + startL);

        TilePos junctAE = new TilePos(placePos.x, placePos.z - 1);
        TilePos carPark1 = new TilePos(placePos.x - 1, placePos.z + 5);
        TilePos carPark2 = new TilePos(placePos.x + 2, placePos.z + 8);
        
        SetJunctionIn(junctAE, EnumDirection.WEST);
        SetJunctionIn(carPark1, EnumDirection.NORTH);
        SetJunctionIn(carPark2, EnumDirection.EAST);
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

    public GenerateHospital(Section section, int buildingId, EnumDirection rotation, TileData building)
        : base(section.GetTilePos(), section.GetSizeX(), 1, 1, section.GetSizeZ()) {
        this.buildingId = buildingId;
        this.rotation = rotation;
        this.building = building;
    }
}