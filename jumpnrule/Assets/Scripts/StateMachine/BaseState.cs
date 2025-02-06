using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public virtual IEnumerator Enter()
    {
        yield break;
    }

    public virtual void Update()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual IEnumerator OnCollisionEnter2D(Collider2D collider)
    {
        yield break;
    }
}
