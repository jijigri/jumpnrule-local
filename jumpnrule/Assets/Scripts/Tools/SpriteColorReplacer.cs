using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorReplacer : MonoBehaviour
{
    [SerializeField] private bool _changeOnAwake = true;
    [SerializeField] private Color _colorToReplace = Color.white;
    [SerializeField] private Color _targetColor = Color.red;

    SpriteRenderer _spriteRenderer;
    Material _material;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_changeOnAwake)
        {
            if (_spriteRenderer != null)
            {
                _material = _spriteRenderer.material;
                _material.SetColor("_ColorToReplace", _colorToReplace);
                _material.SetColor("_TargetColor", _targetColor);
            }
        }
    }

    public void ChangeColor(Color color)
    {
        Debug.Log("Trying to change color");

        if (_spriteRenderer != null)
        {
            if (_material == null)
            {
                _material = _spriteRenderer.material;
            }

            _material.SetColor("_ColorToReplace", _colorToReplace);
            _material.SetColor("_TargetColor", color);
        }
    }
}
