using System;
using Tiles.TileManagement;

public class GenerateOffice : GenerateBuildingBase {
    
    private EnumTile entrance = EnumTile.OFFICE_1_EDGE_RECESSED_ENTRANCE;
    private EnumTile edge = EnumTile.OFFICE_1_EDGE_RECESSED;
    private EnumTile edge_side = EnumTile.OFFICE_1_EDGE;
    private EnumTile corner_l = EnumTile.OFFICE_1_CORNER_RECESSED_L;
    private EnumTile corner_r = EnumTile.OFFICE_1_CORNER_RECESSED_R;
    private EnumTile garage = EnumTile.OFFICE_1_EDGE_RECESSED_GARAGE;
    private EnumTile inner = EnumTile.OFFICE_1_INNER_ROOF;
    
    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        //Most generator classes are relatively similar so this is copy-pasted with slight changes
        //Annoyingly things generate differently enough that most buildings need this custom written.
        
        //Check for entrance, at the front of the building
        if (l == 0) {
            //Arrays are zero-based, so minus 1 where appropriate.
            if (width % 2 == 0) { //Even-width building
                if (w == (width / 2) - 1) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.GetTile(entrance).GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)(width / 2.0)) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.GetTile(entrance).GetId();
                }
            }

            if (w == width - 2) {
                rot = EnumDirection.WEST;
                return TileRegistry.GetTile(garage).GetId();
            }
        }

        if (w == 0) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return TileRegistry.GetTile(corner_l).GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return TileRegistry.GetTile(corner_r).GetId();
            }

            rot = EnumDirection.NORTH;
            return TileRegistry.GetTile(edge_side).GetId();
        }
        if (w == width-1) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return TileRegistry.GetTile(corner_r).GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return TileRegistry.GetTile(corner_l).GetId();
            }
            rot = EnumDirection.SOUTH;
            return TileRegistry.GetTile(edge_side).GetId();
        }

        if (l == 0) {
            rot = EnumDirection.WEST;
            return TileRegistry.GetTile(edge).GetId();
        }

        if (l == length - 1) {
            rot = EnumDirection.EAST;
            return TileRegistry.GetTile(edge).GetId();
        }

        return TileRegistry.GetTile(inner).GetId();
    }
    
    public override void PostGenerate() {}

    public GenerateOffice(TilePos startPos, int width, int minHeight, int maxHeight, int length) 
        : base(startPos, width, minHeight, maxHeight, length) {}
    
    public GenerateOffice(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) 
        : base(startPos, minWidth, maxWidth, minHeight, maxHeight, minLength, maxLength) {}
}