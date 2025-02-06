using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    [SerializeField] private float _disableTime = 5f;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(CRT_Disable());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator CRT_Disable()
    {
        yield return new WaitForSeconds(_disableTime);

        gameObject.SetActive(false);

        yield break;
    }
}
