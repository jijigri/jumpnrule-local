using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildEventReceiver : MonoBehaviour
{
    IHasEventReceiverChild[] _iHasEventReceiverChildren;

    private void Awake()
    {
        _iHasEventReceiverChildren = GetComponentsInParent<IHasEventReceiverChild>();
    }

    public void ReceiveEvent(int id)
    {
        if (_iHasEventReceiverChildren != null)
        {
            foreach(IHasEventReceiverChild child in _iHasEventReceiverChildren)
            {
                child.ReceiveEvent(id);
            }
        }
        else
        {
            Debug.Log("ChildEventReceiver: Coudn't find Event Parent(s)");
        }
    }
}
