using System;
using Tiles.TileManagement;

public class GenerateTownHouses : GenerateBuildingBase {
    
    private EnumTile house = EnumTile.COFFEE_SHOP_1;
    
    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        if (l == 0) {
            //Arrays are zero-based, so minus 1 where appropriate.
            if (width % 2 == 0) { //Even-width building
                if (w == (width / 2) - 1) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.GetTile(house).GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)(width / 2.0)) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.GetTile(house).GetId();
                }
            }

            if (w == width - 2) {
                rot = EnumDirection.WEST;
                return TileRegistry.GetTile(house).GetId();
            }
        }

        if (w == 0) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return -1;
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return -1;
            }

            rot = EnumDirection.NORTH;
            return TileRegistry.GetTile(house).GetId();
        }
        if (w == width-1) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return -1;
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return -1;
            }
            rot = EnumDirection.SOUTH;
            return TileRegistry.GetTile(house).GetId();
        }

        if (l == 0) {
            rot = EnumDirection.WEST;
            return TileRegistry.GetTile(house).GetId();
        }

        if (l == length - 1) {
            rot = EnumDirection.EAST;
            return TileRegistry.GetTile(house).GetId();
        }

        return -1;
        //return PlaceSimpleObject(w, l, ref rot, house);
    }

    public GenerateTownHouses(TilePos startPos, int width, int length) 
        : base(startPos, width, 1, 1, length) {}
    
    public GenerateTownHouses(TilePos startPos, int minWidth, int maxWidth, int minLength, int maxLength) 
        : base(startPos, minWidth, maxWidth, 1, 1, minLength, maxLength) {}
}