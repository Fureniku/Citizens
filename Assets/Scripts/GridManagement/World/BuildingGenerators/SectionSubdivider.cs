using Tiles.TileManagement;

public class SectionSubdivider : GenerateBuildingBase {

    private SubDividerType subType;
    private int spacing;

    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        TilePos pos = GetPosInSection(w, l);
        
        switch (subType) {
            case SubDividerType.CROSS:
                if (w == width / 2 && l == length / 2) {
                    return TileRegistry.CROSSROAD_ROAD_1x1.GetId();
                } else if (w == width / 2) {
                    if (l == 0) PlaceAdditionalRoad(EnumDirection.NORTH, pos);
                    if (l == length-1) PlaceAdditionalRoad(EnumDirection.SOUTH, pos);
                    return TileRegistry.STRAIGHT_ROAD_1x1.GetId();
                    
                } else if (l == length / 2) {
                    if (w == 0) PlaceAdditionalRoad(EnumDirection.EAST, pos);
                    if (w == width - 1) PlaceAdditionalRoad(EnumDirection.WEST, pos);

                    rot = EnumDirection.EAST;
                    return TileRegistry.STRAIGHT_ROAD_1x1.GetId();
                }
                break;
            
            case SubDividerType.HORI_STRIPED:
                if (w % spacing == 0 && w > 1 && w < width-2) {
                    if (l == 0) PlaceAdditionalRoad(EnumDirection.NORTH, pos); 
                    if (l == length-1) PlaceAdditionalRoad(EnumDirection.SOUTH, pos);
                    return TileRegistry.STRAIGHT_ROAD_1x1.GetId();
                }
                break;
            
            case SubDividerType.VERT_STRIPED:
                break;
        }
        return -1;
    }

    private void PlaceAdditionalRoad(EnumDirection direction, TilePos pos) {
        ChunkManager chunkManager = World.Instance.GetChunkManager();
        TilePos offset = Direction.OffsetPos(direction, pos);
        Tile existingTile = chunkManager.GetTile(offset).GetTile();
        if (existingTile.GetTileType() == TileType.ROAD) {
            if (existingTile == TileRegistry.T_JUNCT_ROAD_1x1) {
                chunkManager.SetTile(offset, TileRegistry.CROSSROAD_ROAD_1x1.GetId(), EnumDirection.NORTH);
            } else if (existingTile == TileRegistry.ROAD_WORLD_EDGE_STRAIGHT) {
                chunkManager.SetTile(offset, TileRegistry.ROAD_WORLD_EDGE_T.GetId(), direction.RotateCCW());
            } else {
                chunkManager.SetTile(offset, TileRegistry.T_JUNCT_ROAD_1x1.GetId(), direction.RotateCCW());
            }
        }
    }

    public override void PostGenerate() {}

    public SectionSubdivider(Section section, SubDividerType subType, int spacing = 3)
        : base(section.GetTilePos(), section.GetSizeX(), 1, 1, section.GetSizeZ()) {
        this.subType = subType;
        this.spacing = spacing;
    }
}


public enum SubDividerType {
    CROSS,
    HORI_STRIPED,
    VERT_STRIPED
}