using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] _destroyedSprites;

    bool _isActive = true;

    private SpriteRenderer _spriteRenderer;
    private StudioEventEmitter _audioEmitter;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _audioEmitter = GetComponentInChildren<StudioEventEmitter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isActive)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        if(_destroyedSprites != null)
        {
            _spriteRenderer.sprite = _destroyedSprites[Random.Range(0, _destroyedSprites.Length)];
        }

        ObjectPooler.Instance.SpawnObject("PropBreakEffect", transform.position, Quaternion.identity);

        _audioEmitter.Play();

        _isActive = false;
    }
}
