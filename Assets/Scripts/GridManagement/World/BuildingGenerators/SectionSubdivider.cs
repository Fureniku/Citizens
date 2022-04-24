using Tiles.TileManagement;

public class SectionSubdivider : GenerateBuildingBase {

    private SubDividerType subType;

    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        switch (subType) {
            case SubDividerType.CROSS:
                if (w == width / 2) {
                    return TileRegistry.STRAIGHT_ROAD_1x1.GetId();
                }
                break;
            case SubDividerType.HORI_STRIPED:
                break;
            case SubDividerType.VERT_STRIPED:
                break;
        }
        return -1;
    }
    
    public SectionSubdivider(Section section, SubDividerType subType)
        : base(section.GetTilePos(), section.GetSizeX(), 1, 1, section.GetSizeZ()) {
        this.subType = subType;
    }
}


public enum SubDividerType {
    CROSS,
    HORI_STRIPED,
    VERT_STRIPED
}