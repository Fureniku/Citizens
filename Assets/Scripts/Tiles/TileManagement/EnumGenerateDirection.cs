using System;

public static class GenerateDirection {
    
    public static int GenX(this EnumGenerateDirection dir) {
        switch (dir) {
            case EnumGenerateDirection.NORTH_EAST: return 1;
            case EnumGenerateDirection.SOUTH_EAST: return 1;
            case EnumGenerateDirection.NORTH_WEST: return -1;
            case EnumGenerateDirection.SOUTH_WEST: return -1;
        }
        return 0;
    }
    
    public static int GenZ(this EnumGenerateDirection dir) {
        switch (dir) {
            case EnumGenerateDirection.NORTH_EAST: return 1;
            case EnumGenerateDirection.SOUTH_EAST: return -1;
            case EnumGenerateDirection.NORTH_WEST: return 1;
            case EnumGenerateDirection.SOUTH_WEST: return -1;
        }
        return 0;
    }
    
    public static EnumGenerateDirection GetFromString(string str) {
        foreach (EnumGenerateDirection genDir in Enum.GetValues(typeof(EnumGenerateDirection))) {
            if (str.Equals(genDir.ToString())) {
                return genDir;
            }
        }

        return EnumGenerateDirection.NONE;
    }
}

public enum EnumGenerateDirection {
    NORTH_EAST,
    SOUTH_EAST,
    NORTH_WEST,
    SOUTH_WEST,
    NONE
};