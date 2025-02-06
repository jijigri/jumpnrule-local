using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NeonScript : MonoBehaviour
{
    [SerializeField] [Tooltip("Index of the neon, 0 for a random neon")] private int _neonId = 0;
    [SerializeField] private bool _randomizeOnAwake = false;
    [SerializeField] Light2D _light = null;
    [SerializeField] ColorData _color = null;
    [SerializeField] private List<SpriteRenderer> _neonObjects = new List<SpriteRenderer>();

    private void Awake()
    {
        if (_randomizeOnAwake)
        {
            _neonId = 0;
            UpdateNeonObject();
        }
    }

    private void OnValidate()
    {
        if (_neonObjects == null)
        {
            UpdateNeonList();
        }

        UpdateNeonObject();
    }

    [ContextMenu("Update Neons")]
    void UpdateNeonList()
    {
        _neonObjects.Clear();

        foreach(Transform tr in transform)
        {
            if(tr != null && tr.name != "Light")
            {
                _neonObjects.Add(tr.GetComponent<SpriteRenderer>());
            }
        }
    }

    [ContextMenu("Update Neon Object")]
    void UpdateNeonObject()
    {
        if (_neonId > _neonObjects.Count - 1)
        {
            _neonId = _neonObjects.Count - 1;
        }

        if(_neonId == 0)
        {
            SetNeon(Random.Range(0, _neonObjects.Count));
        }
        else
        {
            SetNeon(_neonId);
        }
    }

    void SetNeon(int index)
    {
        for(int i = 0; i < _neonObjects.Count; i++)
        {
            if(i == index)
            {
                _neonObjects[i].gameObject.SetActive(true);

                if (_light != null)
                {
                    _light.transform.localScale = new Vector3(_neonObjects[i].bounds.size.x, _neonObjects[i].bounds.size.y, 1);

                    if(_color != null)
                    {
                        _light.color = _color.color;
                    }
                }
            }
            else
            {
                _neonObjects[i].gameObject.SetActive(false);
            }
        }
    }
}
