using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoActivateRandomChild : MonoBehaviour
{
    List<Transform> _children;

    void Start()
    {
        SetChildren();
    }

    void SetChildren()
    {
        _children = new List<Transform>();

        foreach (Transform tr in transform)
        {
            if (tr != transform)
            {
                _children.Add(tr);
            }
        }
    }

    private void OnEnable()
    {
        if(_children == null)
        {
            SetChildren();
        }
        if(_children != null)
        {
            foreach(Transform child in _children)
            {
                child.gameObject.SetActive(false);
            }

            ParticleSystem ps = _children[Random.Range(0, _children.Count)].GetComponent<ParticleSystem>();
            ps.Play();
            ps.gameObject.SetActive(true);
        }
    }
}
