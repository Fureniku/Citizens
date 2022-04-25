using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class VehicleJunctionController : MonoBehaviour {

    [SerializeField] private RoadConnectionType up = RoadConnectionType.NOTHING;
    [SerializeField] private RoadConnectionType right  = RoadConnectionType.NOTHING;
    [SerializeField] private RoadConnectionType down = RoadConnectionType.NOTHING;
    [SerializeField] private RoadConnectionType left  = RoadConnectionType.NOTHING;

    private GameObject upIn;
    private GameObject upOut;
    private GameObject rightIn;
    private GameObject rightOut;
    private GameObject downIn;
    private GameObject downOut;
    private GameObject leftIn;
    private GameObject leftOut;

    [SerializeField] private bool prefabNodes = false;
    [SerializeField] private GameObject[] nodes;

    private GameObject nodeParent;

    void Start() {
        nodeParent = new GameObject();
        nodeParent.transform.position = transform.position;
        nodeParent.transform.parent = transform;
        
        nodeParent.name = "Node Parent";
        if (prefabNodes) {
            for (int i = 0; i < nodes.Length; i++) {
                switch (nodes[i].name) {
                    case "UP In": 
                        upIn = nodes[i];
                        upIn.transform.parent = nodeParent.transform;
                        break;
                    case "UP Out":
                        upOut = nodes[i];
                        upOut.transform.parent = nodeParent.transform;
                        break;
                    case "RIGHT In":
                        rightIn = nodes[i];
                        rightIn.transform.parent = nodeParent.transform;
                        break;
                    case "RIGHT Out":
                        rightOut = nodes[i];
                        rightOut.transform.parent = nodeParent.transform;
                        break;
                    case "DOWN In":
                        downIn = nodes[i];
                        downIn.transform.parent = nodeParent.transform;
                        break;
                    case "DOWN Out":
                        downOut = nodes[i];
                        downOut.transform.parent = nodeParent.transform;
                        break;
                    case "LEFT In":
                        leftIn = nodes[i];
                        leftIn.transform.parent = nodeParent.transform;
                        break;
                    case "LEFT Out":
                        leftOut = nodes[i];
                        leftOut.transform.parent = nodeParent.transform;
                        break;
                    
                    default:
                        Debug.Log("Unknown node " + nodes[i].name);
                        break;
                }
            }
        }
        else {
            if (up    != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.NORTH, up);    }
            if (right != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.EAST,  right); }
            if (down  != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.SOUTH, down);  }
            if (left  != RoadConnectionType.NOTHING) { CreateNode(EnumDirection.WEST,  left);  }
        }
        
        nodeParent.transform.localRotation = Quaternion.identity;
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
                upIn = nodeIn;
                upOut = nodeOut;
                break;
            case EnumDirection.EAST:
                rightIn = nodeIn;
                rightOut = nodeOut;
                break;
            case EnumDirection.SOUTH:
                downIn = nodeIn;
                downOut = nodeOut;
                break;
            case EnumDirection.WEST:
                leftIn = nodeIn;
                leftOut = nodeOut;
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

        
        GameObject node = new GameObject();
        node.transform.position = transform.position + (isIn ? new Vector3(xL, y, zL) : new Vector3(xR, y, zR));
        node.transform.parent = nodeParent.transform;
        node.name = dir.GetLocalFromGlobal() + (isIn ? " In" : " Out");
        node.AddComponent<VehicleJunctionNode>();
        VehicleJunctionNode vjNode = node.GetComponent<VehicleJunctionNode>();
        vjNode.SetIsIn(isIn);
        vjNode.SetGiveWay(giveWay);
        return node;
    }

    public GameObject GetInNode(EnumDirection dir) {
        EnumLocalDirection localDir = GetLocalFromEntryPoint(dir, GetComponent<TileData>().GetRotation());
        switch (localDir) {
            case EnumLocalDirection.UP:
                return upIn;
            case EnumLocalDirection.RIGHT:
                return rightIn;
            case EnumLocalDirection.DOWN:
                return downIn;
            case EnumLocalDirection.LEFT:
                return leftIn;
        }

        return upIn;
    }
    
    public GameObject GetOutNode(EnumDirection dir) {
        EnumLocalDirection localDir = GetLocalFromExitPoint(dir, GetComponent<TileData>().GetRotation());
        switch (localDir) {
            case EnumLocalDirection.UP:
                return upOut;
            case EnumLocalDirection.RIGHT:
                return rightOut;
            case EnumLocalDirection.DOWN:
                return downOut;
            case EnumLocalDirection.LEFT:
                return leftOut;
        }

        return upOut;
    }

    private GameObject GetOpposingNode(VehicleJunctionNode nodeIn) {
        VehicleJunctionNode nodeUpIn = upIn != null ? upIn.GetComponent<VehicleJunctionNode>() : null;
        VehicleJunctionNode nodeRightIn = rightIn != null ? rightIn.GetComponent<VehicleJunctionNode>() : null;
        VehicleJunctionNode nodeDownIn = downIn != null ? downIn.GetComponent<VehicleJunctionNode>() : null;
        VehicleJunctionNode nodeLeftIn = leftIn != null ? leftIn.GetComponent<VehicleJunctionNode>() : null;

        if (nodeIn == nodeUpIn) return downOut;
        if (nodeIn == nodeRightIn) return leftOut;
        if (nodeIn == nodeDownIn) return upOut;
        if (nodeIn == nodeLeftIn) return rightOut;

        return null;
    }
    
    private GameObject GetRightNode(VehicleJunctionNode nodeIn) {
        VehicleJunctionNode nodeUpIn = upIn != null ? upIn.GetComponent<VehicleJunctionNode>() : null;
        VehicleJunctionNode nodeRightIn = rightIn != null ? rightIn.GetComponent<VehicleJunctionNode>() : null;
        VehicleJunctionNode nodeDownIn = downIn != null ? downIn.GetComponent<VehicleJunctionNode>() : null;
        VehicleJunctionNode nodeLeftIn = leftIn != null ? leftIn.GetComponent<VehicleJunctionNode>() : null;

        //The right node is opposite to what you might think - based on approaching from down.
        if (nodeIn == nodeUpIn) return leftOut;
        if (nodeIn == nodeRightIn) return upOut;
        if (nodeIn == nodeDownIn) return rightOut;
        if (nodeIn == nodeLeftIn) return downOut;

        return null;
    }

    public bool CrossingJunction(VehicleJunctionNode nodeIn, GameObject nextDest) {
        if (nextDest == null) {
            return false;
        }
        return GetOpposingNode(nodeIn) == nextDest;
    }

    public bool TurningRight(VehicleJunctionNode nodeIn, GameObject nextDest) {
        if (nextDest == null) {
            return false;
        }
        return GetRightNode(nodeIn) == nextDest;
    }

    private bool ShouldGiveWay(VehicleJunctionNode nodeIn, VehicleJunctionNode nodeOut) {
        return false;
    }
    
    private EnumLocalDirection GetLocalFromEntryPoint(EnumDirection entryDirection, EnumDirection tileRotation) {
        switch (entryDirection) {
            case EnumDirection.NORTH:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.UP; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.LEFT; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.DOWN; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.RIGHT; }
                break;
            case EnumDirection.EAST:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.LEFT; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.DOWN; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.RIGHT; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.UP; }
                break;
            case EnumDirection.SOUTH:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.DOWN; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.RIGHT; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.UP; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.LEFT; }
                break;
            case EnumDirection.WEST:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.RIGHT; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.UP; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.LEFT; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.DOWN; }
                break;
        }

        return EnumLocalDirection.UP;
    }
    
    private EnumLocalDirection GetLocalFromExitPoint(EnumDirection exitDirection, EnumDirection tileRotation) {
        switch (exitDirection) {
            case EnumDirection.NORTH:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.DOWN; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.RIGHT; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.UP; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.LEFT; }
                break;
            case EnumDirection.EAST:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.RIGHT; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.UP; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.LEFT; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.DOWN; }
                break;
            case EnumDirection.SOUTH:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.UP; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.LEFT; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.DOWN; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.RIGHT; }
                break;
            case EnumDirection.WEST:
                if (tileRotation == EnumDirection.NORTH) { return EnumLocalDirection.LEFT; }
                if (tileRotation == EnumDirection.EAST)  { return EnumLocalDirection.DOWN; }
                if (tileRotation == EnumDirection.SOUTH) { return EnumLocalDirection.RIGHT; }
                if (tileRotation == EnumDirection.WEST)  { return EnumLocalDirection.UP; }
                break;
        }

        return EnumLocalDirection.UP;
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