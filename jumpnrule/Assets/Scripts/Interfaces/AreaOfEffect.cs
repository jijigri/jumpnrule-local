using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] protected float _radius = 0;

    public virtual void InitializeAOE(float radius)
    {
        _radius = radius;
    }
}
