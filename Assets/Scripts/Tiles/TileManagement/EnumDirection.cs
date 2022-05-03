using UnityEngine;

namespace Tiles.TileManagement {

    public static class Direction {
        
        //Get the int rotation from the enum direction. Remember north/west are negative.
        public static int GetRotation(this EnumDirection dir) {
            switch (dir) {
                case EnumDirection.NORTH: return 180;
                case EnumDirection.EAST:  return 270;
                case EnumDirection.SOUTH: return 0;
                case EnumDirection.WEST:  return 90;
                default: return 0;
            }
        }

        //Get the enum direction from a rotation int.
        public static EnumDirection GetDirection(int dir) {
            switch (dir) {
                case 180: return EnumDirection.NORTH;
                case 270: return EnumDirection.EAST;
                case 0:   return EnumDirection.SOUTH;
                case 90:  return EnumDirection.WEST;
                default: return EnumDirection.NORTH;
            }
        }

        //Rotate a direction clockwise by 90 degrees, returning new direction.
        public static EnumDirection RotateCW(this EnumDirection dirIn) {
            switch (dirIn) {
                case EnumDirection.NORTH: return EnumDirection.EAST;
                case EnumDirection.EAST:  return EnumDirection.SOUTH;
                case EnumDirection.SOUTH: return EnumDirection.WEST;
                case EnumDirection.WEST:  return EnumDirection.NORTH;
                default: return EnumDirection.NORTH;
            }
        }
        
        //Rotate a direction counter-clockwise by 90 degrees, returning new direction
        public static EnumDirection RotateCCW(this EnumDirection dirIn) {
            switch (dirIn) {
                case EnumDirection.NORTH: return EnumDirection.WEST;
                case EnumDirection.EAST:  return EnumDirection.NORTH;
                case EnumDirection.SOUTH: return EnumDirection.EAST;
                case EnumDirection.WEST:  return EnumDirection.SOUTH;
                default: return EnumDirection.NORTH;
            }
        }
        
        //Get the opposing direction
        public static EnumDirection Opposite(this EnumDirection dirIn) {
            switch (dirIn) {
                case EnumDirection.NORTH: return EnumDirection.SOUTH;
                case EnumDirection.EAST:  return EnumDirection.WEST;
                case EnumDirection.SOUTH: return EnumDirection.NORTH;
                case EnumDirection.WEST:  return EnumDirection.EAST;
                default: return EnumDirection.NORTH;
            }
        }

        //Get the X modifier value for this direction (To multiply against a modifier for positive/negative)
        public static int XModify(this EnumDirection dirIn) {
            switch (dirIn) {
                case EnumDirection.EAST:  return -1;
                case EnumDirection.WEST:  return 1;
                default: return 0;
            }
        }
        
        //Get the Z modifier value for this direction (To multiply against a modifier for positive/negative)
        public static int ZModify(this EnumDirection dirIn) {
            switch (dirIn) {
                case EnumDirection.NORTH: return -1;
                case EnumDirection.SOUTH: return 1;
                default: return 0;
            }
        }
        
        //Get the tile position offset X tiles in the direction from the passed position.
        public static TilePos OffsetPos(this EnumDirection dir, TilePos prev, int distance = 1) {
            return new TilePos(prev.x + (distance * dir.XModify()), prev.z + (distance * dir.ZModify()));
        }

        //Get which direction tile B is from tile A
        public static EnumDirection GetDirectionOffset(TilePos origin, TilePos destination) {
            bool n = destination.z < origin.z;
            bool e = destination.x > origin.x;
            bool s = destination.z > origin.z;
            bool w = destination.x < origin.x;

            if (n && !e && !s && !w) return EnumDirection.NORTH;
            if (!n && e && !s && !w) return EnumDirection.EAST;
            if (!n && !e && s && !w) return EnumDirection.SOUTH;
            if (!n && !e && !s && w) return EnumDirection.WEST;
            
            Debug.Log($"Not a straightforward direction. Probably bad. N:{n}, E:{e}, S:{s}, W:{w}");
            return EnumDirection.NORTH;
        }

        public static Vector2 GetDirectionalVector(this EnumDirection dir) {
            switch (dir) {
                case EnumDirection.NORTH: return new Vector2( 0, -1);
                case EnumDirection.EAST:  return new Vector2( 1,  0);
                case EnumDirection.SOUTH: return new Vector2( 0,  1);
                case EnumDirection.WEST:  return new Vector2(-1,  0);
            }
            return Vector2.zero;
        }

        public static string ToString(this EnumDirection dirIn) {
            switch (dirIn) {
                case EnumDirection.NORTH: return "North";
                case EnumDirection.EAST:  return "East";
                case EnumDirection.SOUTH: return "South";
                case EnumDirection.WEST:  return "West";
                default: return "Unknown";
            }
        }

        public static EnumLocalDirection GetLocalFromGlobal(this EnumDirection dirIn) {
            if (dirIn == EnumDirection.NORTH) { return EnumLocalDirection.UP; }
            if (dirIn == EnumDirection.WEST)  { return EnumLocalDirection.LEFT; }
            if (dirIn == EnumDirection.SOUTH) { return EnumLocalDirection.DOWN; }

            return EnumLocalDirection.RIGHT;
        }
    }

    public enum EnumDirection {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    public enum EnumLocalDirection {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}