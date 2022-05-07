using System;
using Tiles.TileManagement;

public class GenerateOffice : GenerateBuildingBase {

    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        //Most generator classes are relatively similar so this is copy-pasted with slight changes
        //Annoyingly things generate differently enough that most buildings need this custom written.
        
        //Check for entrance, at the front of the building
        if (l == 0) {
            //Arrays are zero-based, so minus 1 where appropriate.
            if (width % 2 == 0) { //Even-width building
                if (w == (width / 2) - 1) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.OFFICE_1_EDGE_RECESSED_ENTRANCE.GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)(width / 2.0)) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.OFFICE_1_EDGE_RECESSED_ENTRANCE.GetId();
                }
            }

            if (w == width - 2) {
                rot = EnumDirection.WEST;
                return TileRegistry.OFFICE_1_EDGE_RECESSED_GARAGE.GetId();
            }
        }

        if (w == 0) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return TileRegistry.OFFICE_1_CORNER_RECESSED_L.GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return TileRegistry.OFFICE_1_CORNER_RECESSED_R.GetId();
            }

            rot = EnumDirection.NORTH;
            return TileRegistry.OFFICE_1_EDGE.GetId();
        }
        if (w == width-1) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return TileRegistry.OFFICE_1_CORNER_RECESSED_R.GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return TileRegistry.OFFICE_1_CORNER_RECESSED_L.GetId();
            }
            rot = EnumDirection.SOUTH;
            return TileRegistry.OFFICE_1_EDGE.GetId();
        }

        if (l == 0) {
            rot = EnumDirection.WEST;
            return TileRegistry.OFFICE_1_EDGE.GetId();
        }

        if (l == length - 1) {
            rot = EnumDirection.EAST;
            return TileRegistry.OFFICE_1_EDGE.GetId();
        }

        return TileRegistry.OFFICE_1_INNER_ROOF.GetId();
    }
    
    public override void PostGenerate() {}

    public GenerateOffice(TilePos startPos, int width, int minHeight, int maxHeight, int length) 
        : base(startPos, width, minHeight, maxHeight, length) {}
    
    public GenerateOffice(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) 
        : base(startPos, minWidth, maxWidth, minHeight, maxHeight, minLength, maxLength) {}
}