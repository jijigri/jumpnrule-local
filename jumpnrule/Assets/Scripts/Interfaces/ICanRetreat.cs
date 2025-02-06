using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanRetreat
{
    public void StartRetreating(Vector2 retreatPosition);
    public void StopRetreating();
}
