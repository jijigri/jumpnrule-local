using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    protected BaseState _state;

    public void SetState(BaseState state)
    {
        _state = state;
        StartCoroutine(state.Enter());
    }
}
