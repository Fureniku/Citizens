using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A-star pathfinding code taken from IMAT2904 AI For Simulation provided code.
//All credit Salim Hasshu

public class Node : IHeapElement<Node>
{
    public Vector3 nodePos;
    public int x;
    public int y;
    public bool traversable = false;

    public float g;
    public float h;

    public Node parentNode;

    int heapIndex;

    public Node(Vector3 nodePos, bool traversable, int x, int y)
    {
        this.nodePos = nodePos;
        this.traversable = traversable;
        this.x = x;
        this.y = y;
    }

    public float f
    {
        get
        {
            return g + h;
        }
 
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }


    public int CompareTo(Node nodeCompare)
    {
        int compare = f.CompareTo(nodeCompare.f);
        if (compare == 0)
        {
            compare = h.CompareTo(nodeCompare.h);
        }
        return -compare;
    }
}
