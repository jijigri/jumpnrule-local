using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Material _material;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
    }

    public void Initialize(float size, float ringSize, float magnitude, float speed, float smoothness)
    {
        transform.localScale = new Vector3(size, size, 1);
        _material.SetFloat("_RingSize", ringSize);
        _material.SetFloat("_Magnitude", magnitude);
        _material.SetFloat("_Smoothness", smoothness);

        StopCoroutine(ShockwaveGrow(speed));
        StartCoroutine(ShockwaveGrow(speed));
    }

    IEnumerator ShockwaveGrow(float speed)
    {
        float time = 0;

        while (time < 1)
        {
            _material.SetFloat("_ShaderTime", time);
            time += Time.deltaTime * speed;

            yield return null;
        }

        gameObject.SetActive(false);

        yield break;
    }
}
