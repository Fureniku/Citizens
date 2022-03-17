using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//A-star pathfinding code taken from IMAT2904 AI For Simulation provided code.
//All credit Salim Hasshu

public class AStar : MonoBehaviour
{

    Node[,] grid;
    Vector2 gridSize;
    LayerMask obstacleLayer;
    float nodeSize = 3f;
    Heap<Node> openSet;
    HashSet<Node> closedSet;
    Node currentNode;
    Vector2 gridNodes;
    List<Node> path;
    bool pathFound;
    bool searching = false;
    Vector3 rootNodePos, goalNodePos;

    private void Start()
    {
        //Set grid size based on Gameobject scale (plane)
        gridSize = transform.localScale * 10f;
        //Set gridNodes, how many nodes along x, how many along y
        gridNodes = new Vector2(Mathf.RoundToInt(gridSize.x / nodeSize), Mathf.RoundToInt(gridSize.y / nodeSize));
        //layermark for obstacle search
        obstacleLayer = LayerMask.GetMask("Obstacle");
        //Create grid function
        CreateGrid();
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
                bool traversable = !(Physics.CheckSphere(nodePos, nodeSize / 2, obstacleLayer));

                //Add node to grid.
                grid[i, j] = new Node(nodePos, traversable, i, j);
            }
        }
    }

    //Function uses transform.position to return node in grid matrix.
    public Node NodePositionInGrid(Vector3 gridPosition)
    {
        float pX = Mathf.Clamp01((gridPosition.x - ((gridSize.x / gridNodes.x) / 2) + (gridSize.x / 2)) / gridSize.x);
        float pY = Mathf.Clamp01((gridPosition.z - ((gridSize.y / gridNodes.y) / 2) + (gridSize.y / 2)) / gridSize.y);

        int nX = (int)Mathf.Clamp(Mathf.RoundToInt(gridNodes.x * pX), 0, gridNodes.x - 1);
        int nY = (int)Mathf.Clamp(Mathf.RoundToInt(gridNodes.y * pY), 0, gridNodes.y - 1);

        return grid[nX, nY];
    }



    void AStarPathFind()
    {
        //Get Root Node (ship)
        Node rootNode = NodePositionInGrid(rootNodePos);
        //Get Target Node (Player)
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

    //searchs neighbours
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int pX = node.x + i;
                int pY = node.y + j;

                if (pX >= 0 && pX < gridNodes.x && pY >= 0 && pY < gridNodes.y)
                {
                    neighbours.Add(grid[pX, pY]);
                }
            }
        }
        return neighbours;
    }

    //returns distance between nodeA and nodeB based on heuristic class
    public float GetDistance(Node nodeA, Node nodeB)
    {
        float rValue = 0;
        rValue = Heuristic.GetDistanceEuclidean(nodeA, nodeB);
        return rValue;
    }




    //private void OnDrawGizmos()
    //{

    //    if (grid != null)
    //    {


    //        foreach (Node node in grid)
    //        {

    //            if (node.traversable)
    //            {
    //            }
    //            else
    //            {
    //                Gizmos.DrawCube(node.nodePos, new Vector3(nodeSize * 0.9f, 0.1f, nodeSize * 0.9f));

    //                Gizmos.color = Color.red;
    //            }


    //            if (path != null && pathFound)
    //            {
    //                foreach (var item in path)
    //                {
    //                    if (item == node)
    //                    {
    //                        Gizmos.color = new Color(0, 0, 0.5f, 0.5f);
    //                        Gizmos.DrawCube(node.nodePos, new Vector3(nodeSize * 0.9f, 0.1f, nodeSize * 0.9f));
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}



}
