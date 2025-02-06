using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite = null;
    [SerializeField] private SpriteRenderer _headSprite = null;
    [SerializeField] private ColorData _energyColor = default;
    [SerializeField] private ColorData _healthColor = default;
    [SerializeField] private ColorData _armorColor = default;

    private void Awake()
    {
        Material mat = _sprite.sharedMaterial = new Material(_sprite.material);
        _headSprite.sharedMaterial = mat;
        _sprite.sharedMaterial.SetFloat("_GlintStrength", 0f);
    }

    public void SetGlintEffect(int type = 0)
    {
        switch (type)
        {
            case 0:
                _sprite.sharedMaterial.SetColor("_GlintColor", _energyColor.color);
                break;
            case 1:
                _sprite.sharedMaterial.SetColor("_GlintColor", _healthColor.color);
                break;
            case 2:
                _sprite.sharedMaterial.SetColor("_GlintColor", _armorColor.color);
                break;
        }

        _sprite.sharedMaterial.SetFloat("_GlintStrength", 0f);
        StartCoroutine(CRT_GlintEffect());
    }

    IEnumerator CRT_GlintEffect()
    {
        float t = .99f;

        while(t > 0)
        {
            _sprite.sharedMaterial.SetFloat("_GlintStrength", t);
            t -= Time.deltaTime * 8;

            yield return null;
        }

        _sprite.sharedMaterial.SetFloat("_GlintStrength", 0f);
    }
}
