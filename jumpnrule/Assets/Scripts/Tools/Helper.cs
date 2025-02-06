using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static float AngleBetweenTwoPoints(Vector3 target, Vector3 origin)
    {
        return Mathf.Atan2(target.y - origin.y, target.x - origin.x) * Mathf.Rad2Deg;
    }

    public static Vector2 GetRandomDirection()
    {
        Vector2 direction = Vector2.zero;

        while (direction == Vector2.zero)
        {
            float directionX = UnityEngine.Random.Range(-1f, 1f);
            float directionY = UnityEngine.Random.Range(-1f, 1f);
            direction = new Vector2(directionX, directionY);
        }

        return direction.normalized;
    }

    public static Vector3 ClampMagnitudeMinMax(Vector3 v, float min, float max)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max) return v.normalized * max;
        else if (sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }

    public static GameObject GetKiller(GameObject source)
    {
        GameObject killer = source;

        PlayerOwnedObject playerOwnedObject = source.GetComponent<PlayerOwnedObject>();
        if (playerOwnedObject != null)
        {
            return playerOwnedObject.Owner;
        }
        else
        {
            IHasPlayerOwner playerOwner = source.GetComponent<IHasPlayerOwner>();
            if (playerOwner != null)
            {
                return playerOwner.GetPlayerOwner();
            }
            return killer;
        }
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }


    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }
}
