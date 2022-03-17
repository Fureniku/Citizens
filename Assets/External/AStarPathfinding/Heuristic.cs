using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A-star pathfinding code taken from IMAT2904 AI For Simulation provided code.
//All credit Salim Hasshu

static class Heuristic
{
    public static float GetDistanceEuclidean(Node nodeA, Node nodeB)
    {
        //The difference of nodeA.x and nodeB.x, powered by 2
        float distanceX = Mathf.Pow((nodeB.x - nodeA.x), 2);
        //The difference of nodeA.y and nodeB.y, powered by 2
        float distanceY = Mathf.Pow((nodeB.y - nodeA.y), 2);
        //Sqrt (dX + dY)
        return Mathf.Sqrt(distanceX + distanceY);
    }

    public static float GetDistanceEuclideanNoSqr(Node nodeA, Node nodeB)
    {
        //The difference of nodeA.x and nodeB.x, powered by 2
        float distanceX = Mathf.Pow((nodeB.x - nodeA.x), 2);
        //The difference of nodeA.y and nodeB.y, powered by 2
        float distanceY = Mathf.Pow((nodeB.y - nodeA.y), 2);
        //dx + dy
        return (distanceX + distanceY);
    }

    public static float GetDistanceManhattan(Node nodeA, Node nodeB)
    {
        //The difference of nodeA.x and nodeB.x, absolute value
        int distanceX = Mathf.Abs(nodeB.x - nodeA.x);
        //The difference of nodeA.y and nodeB.y, absolute value
        int distanceY = Mathf.Abs(nodeB.y - nodeA.y);
        //Cost of travel
        int cost = 1;
        //Cost * (dx + dy)
        return cost * (distanceX + distanceY);
    }

    public static float GetDistanceDiag(Node nodeA, Node nodeB)
    {
        //The difference of nodeA.x and nodeB.x, absolute value
        int distanceX = Mathf.Abs(nodeB.x - nodeA.x);
        //The difference of nodeA.y and nodeB.y, absolute value
        int distanceY = Mathf.Abs(nodeB.y - nodeA.y);
        //Cost of travel
        int cost = 1;
        //Cost * The highest distance (dx or dy)
        return cost * Mathf.Max(distanceX, distanceY);
    }
    public static float GetDistanceDiagShort(Node nodeA, Node nodeB)
    {
        //The difference of nodeA.x and nodeB.x, absolute value
        int distanceX = Mathf.Abs(nodeB.x - nodeA.x);
        //The difference of nodeA.y and nodeB.y, absolute value
        int distanceY = Mathf.Abs(nodeB.y - nodeA.y);
        //Cost per travel (diagonal)
        float cost = 1.41f;

        //if dx is larger than dy
        if (distanceX > distanceY)
        {
            //(cost * dy) + (dx - dy)
            return (cost * distanceY) + (distanceX - distanceY);
        }
        else
        {
            //(cost * dx) + (dy - dy)
            return (cost * distanceX) + (distanceY - distanceX);
        }
    }
}
