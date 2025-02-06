using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavPointType
{
    NONE,
    PLATFORM,
    LEFT_EDGE,
    RIGHT_EDGE,
    SOLO,
    BUSY
}

public class NavPoint
{
    public Vector2 tileCoordinates;
    public int platfromIndex;
    public NavPointType navpointType;
    public NavLink leftNavLink;
    public NavLink rightNavLink;
    public NavLink fallNavLink;
    public NavLink jumpNavLink;

    public List<NavPoint> neighbors = new List<NavPoint>();
    public int gCost;
    public int hCost;

    public NavPoint parent;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
