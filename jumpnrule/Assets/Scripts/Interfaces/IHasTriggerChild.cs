using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasTriggerChild
{
    public void OnChildTriggerEnter(Collider2D collision);
}

public interface IHasTriggerExitChild
{
    public void OnChildTriggerExit(Collider2D collision);
}

public interface IHasTriggerStayChild
{
    public void OnChildTriggerStay(Collider2D collision);
}
