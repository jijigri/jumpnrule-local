using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    IHasTriggerChild _parent;
    IHasTriggerExitChild _exitParent;
    IHasTriggerStayChild _stayParent;

    private void Awake()
    {
        _parent = GetComponentInParent<IHasTriggerChild>();
        _exitParent = GetComponentInParent<IHasTriggerExitChild>();
        _stayParent = GetComponentInParent<IHasTriggerStayChild>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_parent != null)
        {
            _parent.OnChildTriggerEnter(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(_exitParent != null)
        {
            _exitParent.OnChildTriggerExit(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_stayParent != null)
        {
            _stayParent.OnChildTriggerStay(collision);
        }
    }
}
