using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

//A-star pathfinding code taken from IMAT2904 AI For Simulation provided code. Modified for use case.
//All credit Salim Hasshu

public class AStar : MonoBehaviour {

    [SerializeField, ReadOnly] private Node[,] grid;
    [SerializeField, ReadOnly] private Vector2 gridSize;
    [SerializeField, ReadOnly] private LayerMask roadLayer;
    [SerializeField, ReadOnly] private float nodeSize = 50f;
    [SerializeField, ReadOnly] private Heap<Node> openSet;
    [SerializeField, ReadOnly] private HashSet<Node> closedSet;
    [SerializeField, ReadOnly] private Node currentNode;
    [SerializeField, ReadOnly] private Vector2 gridNodes;
    [SerializeField, ReadOnly] private List<Node> path;
    [SerializeField, ReadOnly] private bool pathFound;
    [SerializeField, ReadOnly] private bool searching = false;
    [SerializeField, ReadOnly] private Vector3 rootNodePos, goalNodePos;

    [SerializeField] private bool showGizmo = false;

    private void Start() {
        int worldSize = World.Instance.GetGridManager().GetSize();
        nodeSize = World.Instance.GetGridManager().GetGridTileSize();
        float size = worldSize * nodeSize * Chunk.size;
        transform.localScale = new Vector3(5 * worldSize * Chunk.size, 1, 5 * worldSize * Chunk.size);
        transform.position = new Vector3(size / 2, 0.5f, size / 2);
        //Set grid size based on Gameobject scale (plane)
        gridSize = new Vector2(size, size);
        //Set gridNodes, how many nodes along x, how many along y
        gridNodes = new Vector2(Mathf.RoundToInt(gridSize.x / nodeSize), Mathf.RoundToInt(gridSize.y / nodeSize));
        //layermark for obstacle search
        roadLayer = LayerMask.GetMask("Road");
        //Create grid function
        CreateGrid();
        Debug.Log("A Star created!");
    }


    //returns the path from objectA to objectB
    public List<Node> RequestPath(GameObject objectA, GameObject objectB)
    {
        //set rootNodePos position and goalNode
        rootNodePos = objectA.transform.position;
        goalNodePos = objectB.transform.position;

        //redo the grid with CreateGrid function (in case of moved root goal or obstacles)
        //CreateGrid();

        //run path search
        AStarPathFind();

        return path;
    }

    void CreateGrid()
    {
        //Create 2d matrix to store nodes
        grid = new Node[(int)gridNodes.x, (int)gridNodes.y];

        //location bottom left of the grid space
        Vector3 gridBottomLeft = transform.position - Vector3.right * gridSize.x / 2
                                                    - Vector3.forward * gridSize.y / 2;

        //for each node in x axis
        for (int i = 0; i < gridNodes.x; i++)
        {
            //for each node in y axis
            for (int j = 0; j < gridNodes.y; j++)
            {
                //find position in the grid this node needs to be
                Vector3 nodePos = gridBottomLeft + Vector3.right * (i * nodeSize + (nodeSize / 2))
                                                 + Vector3.forward * (j * nodeSize + (nodeSize / 2));


                //cheack for obstacles, is it traverable?
                //Adjacancy Matrix could also be used to add traversability
                bool traversable = (Physics.CheckSphere(nodePos, nodeSize / 2, roadLayer));

                //Add node to grid.
                grid[i, j] = new Node(nodePos, traversable, i, j);
            }
        }
    }

    //Function uses transform.position to return node in grid matrix.
    //Edited to use tile pos functionality instead, more accurate for tiles in a star grid.
    public Node NodePositionInGrid(Vector3 gridPosition) {
        TilePos pos = TilePos.GetGridPosFromLocation(gridPosition);

        return grid[pos.x, pos.z];
    }



    void AStarPathFind()
    {
        Node rootNode = NodePositionInGrid(rootNodePos);
        Node goalNode = NodePositionInGrid(goalNodePos);
        //Create set for open nodes
        openSet = new Heap<Node>(grid.Length);
        //Create set for closed nodes
        closedSet = new HashSet<Node>();
        //Has path been found
        pathFound = false;
        //is searching?
        searching = true;
        //Add root node to open set
        openSet.Add(rootNode);
        //set currentnode variable
        currentNode = new Node(Vector3.zero, false, -1, -1);
        //store new move costs
        float newMoveCost;

        //While there are nodes in the open set and still searching for goal
        while (openSet.Count > 0 && searching)
        {
            //uses heap data structure to find lowest
            currentNode = openSet.RemoveTop();
            //add current node to the closedset
            closedSet.Add(currentNode);

            //if the current node is the goal node, we are finished
            if (currentNode == goalNode)
            {
                //retrace path 
                RetracePath(rootNode, goalNode);
                pathFound = true;
                searching = false;
                break;
            }
            else
            {
                //for each neighbour of current node
                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    //if we cannot traverse to the neighbour, or is in the close set
                    if (!neighbour.traversable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //Calculate the move to neightbor gCost
                    newMoveCost = currentNode.g + GetDistance(currentNode, neighbour);

                    //if the new move costs less or this neighbour isnt in the open set
                    if (newMoveCost < neighbour.g || !openSet.Contains(neighbour))
                    {
                        //store (new) move cost
                        neighbour.g = newMoveCost;
                        //store heuristic hCost from neighbour to goal
                        neighbour.h = GetDistance(neighbour, goalNode);
                        //store parent node for path retrace
                        neighbour.parentNode = currentNode;

                        //if we dont have this neighbour in the open set
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        searching = false;
    }

    //returns path
    void RetracePath(Node rNode, Node gNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = gNode;

        while (currentNode != rNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        path.Reverse();
        if(path != null)
        {
            this.path = path;

        }
    }

    //searchs neighbours (Changed to only be cardinal directions, no diagonals)
    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        int left  = node.x-1;
        int right = node.x+1;
        int up    = node.y-1;
        int down  = node.y+1;
        
        if (ValidNeighbour(left, node.y)) neighbours.Add(grid[left, node.y]);
        if (ValidNeighbour(right, node.y)) neighbours.Add(grid[right, node.y]);
        if (ValidNeighbour(node.x, up)) neighbours.Add(grid[node.x, up]);
        if (ValidNeighbour(node.x, down)) neighbours.Add(grid[node.x, down]);
        
        return neighbours;
    }

    private bool ValidNeighbour(int x, int y) {
        return x >= 0 && x < gridNodes.x && y >= 0 && y < gridNodes.y;
    }

    //returns distance between nodeA and nodeB based on heuristic class
    public float GetDistance(Node nodeA, Node nodeB)
    {
        float rValue = 0;
        rValue = Heuristic.GetDistanceEuclidean(nodeA, nodeB);
        return rValue;
    }




    private void OnDrawGizmos()
    {
        if (showGizmo) {
            if (grid != null)
            {
                foreach (Node node in grid)
                {
                    if (node.traversable)
                    {
                    }
                    else
                    {
                        Gizmos.DrawCube(node.nodePos, new Vector3(nodeSize * 0.9f, 0.1f, nodeSize * 0.9f));

                        Gizmos.color = Color.red;
                    }
                    
                    if (path != null && pathFound)
                    {
                        foreach (var item in path)
                        {
                            if (item == node)
                            {
                                Gizmos.color = new Color(0, 0, 0.5f, 0.5f);
                                Gizmos.DrawCube(node.nodePos, new Vector3(nodeSize * 0.9f, 0.1f, nodeSize * 0.9f));
                            }
                        }
                    }
                }
            }
        }
    }
}
