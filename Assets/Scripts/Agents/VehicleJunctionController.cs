using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class VehicleJunctionController : MonoBehaviour {

    [SerializeField] private RoadConnectionType north = RoadConnectionType.NOTHING;
    [SerializeField] private RoadConnectionType east  = RoadConnectionType.NOTHING;
    [SerializeField] private RoadConnectionType south = RoadConnectionType.NOTHING;
    [SerializeField] private RoadConnectionType west  = RoadConnectionType.NOTHING;

    private GameObject northIn;
    private GameObject northOut;
    private GameObject eastIn;
    private GameObject eastOut;
    private GameObject southIn;
    private GameObject southOut;
    private GameObject westIn;
    private GameObject westOut;

    void Start() {
        if (north != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.NORTH, north); }
        if (east  != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.EAST,  east);  }
        if (south != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.SOUTH, south); }
        if (west  != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.WEST,  west);  }
    }

    //Create in/out nodes on the given side for the given type
    private void CreateNode(EnumDirection dir, RoadConnectionType type) {
        GameObject nodeIn = null;
        GameObject nodeOut = null;
        
        switch (type) {
            case RoadConnectionType.ROAD_TWO_WAY:
                nodeIn = CreateNodeGO(dir, true, false);
                nodeOut = CreateNodeGO(dir, false, false);
                break;
            case RoadConnectionType.ROAD_IN:
                nodeIn = CreateNodeGO(dir, true, false);
                break;
            case RoadConnectionType.ROAD_OUT:
                nodeOut = CreateNodeGO(dir, false, false);
                break;
            case RoadConnectionType.JUNCT_TWO_WAY:
                nodeIn = CreateNodeGO(dir, true, true);
                nodeOut = CreateNodeGO(dir, false, false);
                break;
            case RoadConnectionType.JUNCT_IN:
                nodeIn = CreateNodeGO(dir, true, true);
                break;
            case RoadConnectionType.JUNCT_OUT:
                nodeOut = CreateNodeGO(dir, false, false);
                break;
        }
        
        switch (dir) {
            case EnumDirection.NORTH:
                northIn = nodeIn;
                northOut = nodeOut;
                break;
            case EnumDirection.EAST:
                eastIn = nodeIn;
                eastOut = nodeOut;
                break;
            case EnumDirection.SOUTH:
                southIn = nodeIn;
                southOut = nodeOut;
                break;
            case EnumDirection.WEST:
                westIn = nodeIn;
                westOut = nodeOut;
                break;
        }
    }

    private GameObject CreateNodeGO(EnumDirection dir, bool isIn, bool giveWay) {
        float xL = 0;
        float xR = 0;
        float zL = 0;
        float zR = 0;
        float y = transform.position.y;

        switch (dir) { //Find the location for the nodes
            case EnumDirection.NORTH:
                xL = -1.6f;
                xR = 1.6f;
                zL = -5f;
                zR = -5f;
                break;
            case EnumDirection.EAST:
                xL = -5f;
                xR = -5f;
                zL = 1.6f;
                zR = -1.6f;
                break;
            case EnumDirection.SOUTH:
                xL = 1.6f;
                xR = -1.6f;
                zL = 5f;
                zR = 5f;
                break;
            case EnumDirection.WEST:
                xL = 5f;
                xR = 5f;
                zL = -1.6f;
                zR = 1.6f;
                break;
        }

        Vector3 localScale = transform.localScale;
        
        xL *= localScale.x;
        xR *= localScale.x;
        zL *= localScale.x;
        zR *= localScale.x;
        
        GameObject node = new GameObject();
        node.transform.position = transform.position + (isIn ? new Vector3(xL, y, zL) : new Vector3(xR, y, zR));
        node.transform.parent = transform;
        node.name = dir + (isIn ? " In" : " Out");
        node.AddComponent<VehicleJunctionNode>();
        VehicleJunctionNode vjNode = node.GetComponent<VehicleJunctionNode>();
        vjNode.SetIsIn(isIn);
        vjNode.SetGiveWay(giveWay);
        return node;
    }

    public GameObject GetInNode(EnumDirection dir) {
        switch (dir) {
            case EnumDirection.NORTH:
                return northIn;
            case EnumDirection.EAST:
                return eastIn;
            case EnumDirection.SOUTH:
                return southIn;
            case EnumDirection.WEST:
                return westIn;
        }

        return northIn;
    }
    
    public GameObject GetOutNode(EnumDirection dir) {
        switch (dir) {
            case EnumDirection.NORTH:
                return northOut;
            case EnumDirection.EAST:
                return eastOut;
            case EnumDirection.SOUTH:
                return southOut;
            case EnumDirection.WEST:
                return westOut;
        }

        return northOut;
    }

    private bool ShouldGiveWay(VehicleJunctionNode nodeIn, VehicleJunctionNode nodeOut) {
        return false;
    }
}

public enum RoadConnectionType {
    ROAD_TWO_WAY,
    ROAD_IN,
    ROAD_OUT,
    JUNCT_TWO_WAY,
    JUNCT_IN,
    JUNCT_OUT,
    NOTHING
}
