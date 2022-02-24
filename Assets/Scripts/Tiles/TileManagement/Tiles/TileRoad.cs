using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRoad : TileData {

    [SerializeField] private RoadType roadType = RoadType.ROAD_STRAIGHT;
    
    void Start() {
        switch (roadType) {
            case RoadType.ROAD_STRAIGHT:
                tileId = TileRegistry.STRAIGHT_ROAD_1x1.GetId();
                break;
            case RoadType.ROAD_CORNER:
                tileId = TileRegistry.CORNER_ROAD_1x1.GetId();
                break;
            case RoadType.ROAD_T_JUNCT:
                tileId = TileRegistry.T_JUNCT_ROAD_1x1.GetId();
                break;
            case RoadType.ROAD_CROSSROAD:
                tileId = TileRegistry.CROSSROAD_ROAD_1x1.GetId();
                break;
            case RoadType.ROAD_CROSSROAD_CONTROLLED:
                tileId = TileRegistry.CROSSROAD_CTRL_ROAD_1x1.GetId();
                break;
        }
        tileId = TileRegistry.GRASS.GetId();
        width = 1;
        length = 1;
    }
}

public enum RoadType {
    ROAD_STRAIGHT,
    ROAD_CORNER,
    ROAD_T_JUNCT,
    ROAD_CROSSROAD,
    ROAD_CROSSROAD_CONTROLLED
}
