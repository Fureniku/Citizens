using UnityEngine;

namespace Tiles.TileManagement {

    public static class Direction {
        public static int GetRotation(this EnumTileDirection dir) {
            switch (dir) {
                case EnumTileDirection.NORTH: return 180;
                case EnumTileDirection.EAST:  return 270;
                case EnumTileDirection.SOUTH: return 0;
                case EnumTileDirection.WEST:  return 90;
                default: return 0;
            }
        }

        public static EnumTileDirection GetDirection(int dir) {
            switch (dir) {
                case 0:   return EnumTileDirection.NORTH;
                case 90:  return EnumTileDirection.NORTH;
                case 180: return EnumTileDirection.NORTH;
                case 270: return EnumTileDirection.NORTH;
                default: return EnumTileDirection.NORTH;
            }
        }

        // ReSharper disable once InconsistentNaming
        public static EnumTileDirection RotateCW(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return EnumTileDirection.EAST;
                case EnumTileDirection.EAST:  return EnumTileDirection.SOUTH;
                case EnumTileDirection.SOUTH: return EnumTileDirection.WEST;
                case EnumTileDirection.WEST:  return EnumTileDirection.NORTH;
                default: return EnumTileDirection.NORTH;
            }
        }
        
        // ReSharper disable once InconsistentNaming
        public static EnumTileDirection RotateCCW(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return EnumTileDirection.WEST;
                case EnumTileDirection.EAST:  return EnumTileDirection.NORTH;
                case EnumTileDirection.SOUTH: return EnumTileDirection.EAST;
                case EnumTileDirection.WEST:  return EnumTileDirection.SOUTH;
                default: return EnumTileDirection.NORTH;
            }
        }
        
        public static EnumTileDirection Opposite(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return EnumTileDirection.SOUTH;
                case EnumTileDirection.EAST:  return EnumTileDirection.WEST;
                case EnumTileDirection.SOUTH: return EnumTileDirection.NORTH;
                case EnumTileDirection.WEST:  return EnumTileDirection.EAST;
                default: return EnumTileDirection.NORTH;
            }
        }

        public static int XModify(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.EAST:  return -1;
                case EnumTileDirection.WEST:  return 1;
                default: return 0;
            }
        }
        
        public static int ZModify(this EnumTileDirection dirIn) {
            switch (dirIn) {
                case EnumTileDirection.NORTH: return -1;
                case EnumTileDirection.SOUTH: return 1;
                default: return 0;
            }
        }
    }

    public enum EnumTileDirection {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
}