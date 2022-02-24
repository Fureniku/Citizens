using UnityEngine;

namespace Tiles.TileManagement {

    public static class Direction {
        
        //Get the int rotation from the enum direction. Remember north/west are negative.
        public static int GetRotation(this EnumTileDirection dir) {
            switch (dir) {
                case EnumTileDirection.NORTH: return 180;
                case EnumTileDirection.EAST:  return 270;
                case EnumTileDirection.SOUTH: return 0;
                case EnumTileDirection.WEST:  return 90;
                default: return 0;
            }
        }

        //Get the enum direction from a rotation int.
        public static EnumTileDirection GetDirection(int dir) {
            switch (dir) {
                case 180: return EnumTileDirection.NORTH;
                case 270: return EnumTileDirection.EAST;
                case 0:   return EnumTileDirection.SOUTH;
                case 90:  return EnumTileDirection.WEST;
                default: return EnumTileDirection.NORTH;
            }
        }

        //Rotate a direction clockwise by 90 degrees, returning new direction.
        public static EnumTileDirection RotateCW(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return EnumTileDirection.EAST;
                case EnumTileDirection.EAST:  return EnumTileDirection.SOUTH;
                case EnumTileDirection.SOUTH: return EnumTileDirection.WEST;
                case EnumTileDirection.WEST:  return EnumTileDirection.NORTH;
                default: return EnumTileDirection.NORTH;
            }
        }
        
        //Rotate a direction counter-clockwise by 90 degrees, returning new direction
        public static EnumTileDirection RotateCCW(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return EnumTileDirection.WEST;
                case EnumTileDirection.EAST:  return EnumTileDirection.NORTH;
                case EnumTileDirection.SOUTH: return EnumTileDirection.EAST;
                case EnumTileDirection.WEST:  return EnumTileDirection.SOUTH;
                default: return EnumTileDirection.NORTH;
            }
        }
        
        //Get the opposing direction
        public static EnumTileDirection Opposite(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return EnumTileDirection.SOUTH;
                case EnumTileDirection.EAST:  return EnumTileDirection.WEST;
                case EnumTileDirection.SOUTH: return EnumTileDirection.NORTH;
                case EnumTileDirection.WEST:  return EnumTileDirection.EAST;
                default: return EnumTileDirection.NORTH;
            }
        }

        //Get the X modifier value for this direction (To multiply against a modifier for positive/negative)
        public static int XModify(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.EAST:  return -1;
                case EnumTileDirection.WEST:  return 1;
                default: return 0;
            }
        }
        
        //Get the Z modifier value for this direction (To multiply against a modifier for positive/negative)
        public static int ZModify(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return -1;
                case EnumTileDirection.SOUTH: return 1;
                default: return 0;
            }
        }
        
        //Get the tile position offset X tiles in the direction from the passed position.
        public static TilePos OffsetPos(this EnumTileDirection dir, TilePos prev, int distance = 1) {
            return new TilePos(prev.x + (distance * dir.XModify()), prev.z + (distance * dir.ZModify()));
        }
    }

    public enum EnumTileDirection {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
}