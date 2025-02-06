using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavMeshGeneration : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private LayerMask _walkableMask;
    [SerializeField] private LayerMask _solidMask;
    public Sprite debugSprite;

    public NavPoint[,] NavPoints { get; private set; }

    public List<Vector2> FreePositions { get; private set; } = new List<Vector2>();

    private void Start()
    {
        GenerateMap();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        NavPoints = new NavPoint[(int)_mapSize.x, (int)_mapSize.y];

        int actualPlatformIndex = 0;
        bool platformStarted = false;

        for (int y = 0; y < _mapSize.y; y++)
        {
            for(int x = 0; x < _mapSize.x; x++)
            {
                Vector2 checkPosition = new Vector2((transform.position.x + x) - (_mapSize.x / 2), (transform.position.y + y) - (_mapSize.y / 2));
                NavPointType type = NavPointType.NONE;

                bool isTileBusy = CheckTileCollision(checkPosition);
                bool isBottomTileBusy = CheckTileCollision(checkPosition + Vector2.down);

                //IF PLATFORM ISN'T STARTED, ALWAYS LEFT EDGE
                if (!platformStarted)
                {
                    //IF THE CURRENT TILE IS A PLATFORM TILE (BOTTOM IS BUSY, TILE IS FREE)
                    if (!isTileBusy && isBottomTileBusy)
                    {
                        bool isRightBusy = CheckTileCollision(checkPosition + Vector2.right);
                        bool isBottomRightBusy = CheckTileCollision(checkPosition + Vector2.down + Vector2.right);

                        //IF RIGHT IS NOT A PLATFORM, NOT AN EDGE, JUST A SOLO PLATFORM
                        if (isRightBusy || !isBottomRightBusy)
                        {
                            type = NavPointType.SOLO;
                            platformStarted = false;
                        }
                        //IF RIGHT CONTINUES THE PLATFORM, LEFT EDGE
                        else
                        {
                            type = NavPointType.LEFT_EDGE;
                            platformStarted = true;
                        }
                    }
                    actualPlatformIndex += 1;
                }
                else
                {
                    bool isRightBusy = CheckTileCollision(checkPosition + Vector2.right);
                    bool isBottomRightBusy = CheckTileCollision(checkPosition + Vector2.down + Vector2.right);

                    if(!isRightBusy && isBottomRightBusy)
                    {
                        type = NavPointType.PLATFORM;
                    }
                    else
                    {
                        type = NavPointType.RIGHT_EDGE;
                        platformStarted = false;
                    }
                }

                if(type != NavPointType.BUSY && type != NavPointType.NONE)
                {
                    FreePositions.Add(checkPosition);
                }

                NavPoints[x, y] = new NavPoint { tileCoordinates = checkPosition, navpointType = type, platfromIndex = actualPlatformIndex };
            }
        }

        GenerateNeighbors();
    }

    void GenerateNeighbors()
    {
        int index = 0;
        foreach(NavPoint point in NavPoints)
        {
            if(point.navpointType == NavPointType.NONE)
            {
                continue;
            }

            foreach (NavPoint other in NavPoints)
            {
                if(point == other || other.navpointType == NavPointType.NONE)
                {
                    continue;
                }

                if (point.tileCoordinates.y != other.tileCoordinates.y)
                {
                    int maxVerticalDistance = 30;

                    //If it's a drop, higher max distance
                    if(other.tileCoordinates.y < point.tileCoordinates.y)
                    {
                        maxVerticalDistance = 40;
                    }

                    float distanceBetweenPoints = Vector2.Distance(point.tileCoordinates, other.tileCoordinates);
                    if (distanceBetweenPoints <= maxVerticalDistance)
                    {
                        /*
                        if (other.tileCoordinates.y < point.tileCoordinates.y)
                        {
                            if (other.tileCoordinates.y > point.tileCoordinates.y)
                            {
                                if (CanJump(point.tileCoordinates, other.tileCoordinates, new Vector2(.9f, 1.9f)))
                                {
                                    point.neighbors.Add(other);
                                }
                            }
                        }
                        else
                        {
                            Vector2 direction = other.tileCoordinates - point.tileCoordinates;
                            direction.Normalize();

                            RaycastHit2D hit = Physics2D.Raycast(point.tileCoordinates, direction, distanceBetweenPoints, _solidMask);

                            if (!hit)
                            {
                                point.neighbors.Add(other);
                            }
                        } */
                        Vector2 direction = other.tileCoordinates - point.tileCoordinates;
                        direction.Normalize();

                        RaycastHit2D hit = Physics2D.Raycast(point.tileCoordinates, direction, distanceBetweenPoints, _solidMask);

                        if (!hit)
                        {
                            point.neighbors.Add(other);
                        }
                    }
                }
                else
                {
                    if (Vector2.Distance(point.tileCoordinates, other.tileCoordinates) <= 1)
                    {
                        Vector2 direction = other.tileCoordinates - point.tileCoordinates;
                        direction.Normalize();

                        bool hit = Physics2D.Raycast(point.tileCoordinates, direction, 1, _solidMask);

                        if (!hit)
                        {
                            point.neighbors.Add(other);
                        }
                    }
                }
            }
            index++;
        }
    }

    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        NavPoints = new NavPoint[(int)_mapSize.x,(int)_mapSize.y];
    }

    bool CheckTileCollision(Vector2 position)
    {
        bool isCollision = Physics2D.OverlapBox(position + new Vector2(0, .2f), Vector2.one * 1f, 0, _walkableMask);

        return isCollision;
    }

    public List<NavPoint> GetNeighbors(NavPoint point)
    {
        return point.neighbors;
    }

    bool CanJump(Vector2 startPos, Vector2 endPos, Vector2 hitboxSize)
    {
        bool canJump = true;

        float yDistance = endPos.y - startPos.y;
        float jumpHeight = Mathf.Clamp(yDistance / 2, 1, 1000);
        int jumpPrecision = 10;
        Vector2[] jumpPath = new Vector2[jumpPrecision + 1];

        bool canDebug = false;
        if (startPos == new Vector2(4.5f, -10.5f)) canDebug = false;

        float offset = .5f;

        for (int t = 0; t <= jumpPrecision; t++)
        {
            jumpPath[t] = Helper.Parabola(startPos, endPos, jumpHeight, (float)t / (float)jumpPrecision);

            bool isColliding = Physics2D.OverlapBox(jumpPath[t] + new Vector2(0, offset), hitboxSize, 0, _solidMask);

            if (isColliding)
            {
                canJump = false;
            }

            if (canDebug)
            {
                GameObject debug = new GameObject("Debug");
                debug.AddComponent<SpriteRenderer>().sprite = debugSprite;
                debug.transform.localScale = new Vector3(hitboxSize.x, hitboxSize.y, 1);
                debug.transform.position = jumpPath[t] + new Vector2(0, offset);
            }
        }

        return canJump;
    }

    public NavPoint WorldPositionToNavPoint(Vector2 worldPosition)
    {
        float currentMinDistance = 200;
        NavPoint closestPoint = null;

        /*
        RaycastHit2D groundCheck = Physics2D.Raycast(worldPosition + new Vector2(0, .5f), Vector2.down, 20f, _walkableMask);
        if (groundCheck)
        {
            worldPosition = groundCheck.point + (Vector2.up * .5f);
        }
        */

        foreach (NavPoint point in NavPoints)
        {
            if (point.navpointType == NavPointType.NONE)
            {
                continue;
            }

            if(Vector2.Distance(worldPosition, point.tileCoordinates) < currentMinDistance)
            {
                currentMinDistance = Vector2.Distance(worldPosition, point.tileCoordinates);
                closestPoint = point;
            }
        }

        /*
        GameObject debug = new GameObject("Debug");
        debug.AddComponent<SpriteRenderer>().sprite = debugSprite;
        debug.transform.position = closestPoint.tileCoordinates;
        */

        return closestPoint;
    }

    private void OnDrawGizmosSelected()
    {
        if(NavPoints == null)
        {
            return;
        }

        foreach(NavPoint point in NavPoints)
        {
            if(point == null)
            {
                return;
            }

            if (point.navpointType != NavPointType.NONE)
            {
                float sphereSize = .25f;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(point.tileCoordinates, sphereSize);
            }

            foreach(NavPoint neighbor in point.neighbors)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(point.tileCoordinates, neighbor.tileCoordinates);
            }
        }
    }
}
