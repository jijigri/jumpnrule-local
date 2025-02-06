using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIPanel : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _closeAnimationTime = .25f;

    private void OnEnable()
    {
        CancelInvoke();
    }

    public void ClosePanel()
    {
        _animator.SetTrigger("close");
        Invoke("DisablePanel", _closeAnimationTime);
    }

    void DisablePanel()
    {
        gameObject.SetActive(false);
    }
}
