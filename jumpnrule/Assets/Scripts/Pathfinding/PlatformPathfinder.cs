using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformPathfinder : MonoBehaviour
{
    public Transform seeker, target;

    PlatformPathRequestManager requestManager;
    public NavMeshGeneration grid;

    private void Awake()
    {
        requestManager = GetComponent<PlatformPathRequestManager>();
    }

    private void Update()
    {
        //FindPath(seeker.position, target.position);
    }

    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    {
        WayPoint[] wayPoints = new WayPoint[0];
        bool pathSuccess = false;

        NavPoint startPoint = grid.WorldPositionToNavPoint(startPos);
        NavPoint targetPoint = grid.WorldPositionToNavPoint(targetPos);

        /*
        Console.Clear();
        Debug.Log("START POINT: " + startPoint.tileCoordinates);
        Debug.Log("TARGET POINT: " + targetPoint.tileCoordinates);
        */

        List<NavPoint> openSet = new List<NavPoint>();
        HashSet<NavPoint> closedSet = new HashSet<NavPoint>();

        openSet.Add(startPoint);

        int attempt = 0;
        while(openSet.Count > 0)
        {
            NavPoint currentPoint = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentPoint.fCost || openSet[i].fCost == currentPoint.fCost && openSet[i].hCost < currentPoint.hCost)
                {
                    currentPoint = openSet[i];
                }
            }

            /*
            Debug.Log("CURRENT POINT : " + currentPoint.tileCoordinates + " HAS NEIGHBORS : " + currentPoint.neighbors.Count);
            Debug.Log("CURRENT POINT : " + currentPoint.tileCoordinates + " HAS GRID NEIGHBORS : " + grid.GetNeighbors(currentPoint).Count);
            */

            openSet.Remove(currentPoint);
            closedSet.Add(currentPoint);

            if(currentPoint == targetPoint)
            {
                pathSuccess = true;
                break;
            }

            foreach(NavPoint neighbor in grid.GetNeighbors(currentPoint))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentPoint.gCost + GetDistance(currentPoint, neighbor);
                if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetPoint);
                    neighbor.parent = currentPoint;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

            attempt++;
            if(attempt > 50000)
            {
                Debug.Log("Open Set couldn't be emptied");
                break;
            }
        }

        yield return null;

        if (pathSuccess)
        {
            wayPoints = RetracePath(startPoint, targetPoint);
        }

        for(int i = 0; i < wayPoints.Length; i++)
        {
            Vector2 point = wayPoints[i].position;
            /*
            GameObject obj = new GameObject("Point");
            obj.transform.position = point;
            */
        }

        requestManager.FinishedProcessingPath(wayPoints, pathSuccess);
    }

    WayPoint[] RetracePath(NavPoint startPoint, NavPoint endPoint)
    {
        List<NavPoint> path = new List<NavPoint>();
        NavPoint currentPoint = endPoint;

        while (currentPoint != startPoint)
        {
            path.Add(currentPoint);
            
            currentPoint = currentPoint.parent;
        }
        WayPoint[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    WayPoint[] SimplifyPath(List <NavPoint> path)
    {
        List<WayPoint> waypoints = new List<WayPoint>();
        Vector2 directionOld = Vector2.zero;

        if (path.Count > 0)
        {
            waypoints.Add(new WayPoint(path[0].tileCoordinates));

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].tileCoordinates.x - path[i].tileCoordinates.x, path[i - 1].tileCoordinates.y - path[i].tileCoordinates.y);
                if (directionNew != directionOld)
                {
                }

                WayPointType type = WayPointType.RUN;

                if(path[i].tileCoordinates.y > path[i - 1].tileCoordinates.y)
                {
                    type = WayPointType.JUMP;
                }
                else if(path[i].tileCoordinates.y < path[i - 1].tileCoordinates.y)
                {
                    type = WayPointType.FALL;
                }

                waypoints.Add(new WayPoint(path[i].tileCoordinates, type));

                directionOld = directionNew;
            }
        }

        return waypoints.ToArray();
    }

    int GetDistance(NavPoint pointA, NavPoint pointB)
    {
        return Mathf.RoundToInt(Vector2.Distance(pointA.tileCoordinates, pointB.tileCoordinates));
    }
}

public struct WayPoint
{
    public Vector2 position;
    public WayPointType type;

    public WayPoint(Vector2 position)
    {
        this.position = position;
        type = WayPointType.RUN;
    }

    public WayPoint(Vector2 position, WayPointType type)
    {
        this.position = position;
        this.type = type;
    }
}

public enum WayPointType
{
    RUN,
    JUMP,
    FALL
}
