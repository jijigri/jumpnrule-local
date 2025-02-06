using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorPalette
{
    public Color backgroundColor;
    public Color foregroundColor;
}

public class BackgroundEffectsManager : MonoBehaviour
{
    public static BackgroundEffectsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private ColorPalette damagePalette;
    [SerializeField] private ColorPalette healPalette;
    [SerializeField] private ColorPalette _goldPalette;
    [SerializeField] private Color _criticalTint = default;
    [SerializeField] SpriteRenderer _spriteRenderer = null;
    [SerializeField] private MeshRenderer _screenQuad = null;
    Material _material;

    Material _deathEffectMaterial;

    bool _isCritical = false;

    private void Start()
    {
        GameObject[] backgrounds = GameObject.FindGameObjectsWithTag("Background");
        if(backgrounds.Length > 0)
        {
            for(int i = 0; i < backgrounds.Length; i++)
            {
                SpriteRenderer spriteRenderer = backgrounds[i].GetComponent<SpriteRenderer>();
                spriteRenderer.sharedMaterial = new Material(_spriteRenderer.material);
                _material = spriteRenderer.sharedMaterial;
            }
        }

        if(_screenQuad != null)
        {
            _screenQuad.sharedMaterial = new Material(_screenQuad.material);
        }
    }

    private void Update()
    {
        if (_isCritical)
        {
            float value = Mathf.Cos(Time.time * 4);

            _spriteRenderer.sharedMaterial.SetFloat("_Brightness", value / 2);
            _spriteRenderer.sharedMaterial.SetFloat("_TintValue", 1f);
        }
        else
        {
            _spriteRenderer.sharedMaterial.SetFloat("_Brightness", 0f);
            _spriteRenderer.sharedMaterial.SetFloat("_TintValue", 0f);
        }
    }

    public void DamageEffect(float fadeIn, float fadeOut, float strength = 1f)
    {
        //StopAllCoroutines();

        SetColorPalette(0);
        StartCoroutine(CRT_Effect(fadeIn, fadeOut, strength));
    }

    public void HealEffect(float fadeIn, float fadeOut, float strength = 1f)
    {
        //StopAllCoroutines();

        SetColorPalette(1);
        StartCoroutine(CRT_Effect(fadeIn, fadeOut, strength));
    }

    public void GoldEffect(float fadeIn, float fadeOut, float strength = 1f)
    {
        //StopAllCoroutines();

        SetColorPalette(2);
        StartCoroutine(CRT_Effect(fadeIn, fadeOut, strength));
    }

    IEnumerator CRT_Effect(float fadeIn, float fadeOut, float strength)
    {
        _spriteRenderer.sharedMaterial.SetFloat("_ShaderValue", 0);

        float elaspedTime = 0;
        float t = elaspedTime;

        while (elaspedTime < fadeIn)
        {
            elaspedTime += Time.deltaTime;

            t = elaspedTime / fadeIn;
            float value = Mathf.Lerp(0, 1, t);
            value *= strength;
            _spriteRenderer.sharedMaterial.SetFloat("_ShaderValue", value);

            yield return null;
        }

        elaspedTime = 0;
        t = 0;
        while (elaspedTime < fadeOut)
        {
            elaspedTime += Time.deltaTime;

            t = elaspedTime / fadeOut;
            float value = Mathf.Lerp(1, 0, t);
            value *= strength;
            _spriteRenderer.sharedMaterial.SetFloat("_ShaderValue", value);

            yield return null;
        }

        _spriteRenderer.sharedMaterial.SetFloat("_ShaderValue", 0);

        yield break;
    }

    public void SetCriticalEffect()
    {
        if(_isCritical == false)
        {
            _spriteRenderer.sharedMaterial.SetColor("_Tint", _criticalTint);
        }

        _isCritical = true;
    }

    public void StopCriticalEffect()
    {
        _isCritical = false;
    }

    void SetColorPalette(int typeId)
    {
        switch (typeId)
        {
            //DAMAGE
            case 0:
                _spriteRenderer.sharedMaterial.SetColor("_TargetColor1", damagePalette.backgroundColor);
                _spriteRenderer.sharedMaterial.SetColor("_TargetColor2", damagePalette.foregroundColor);
                break;
            //HEAL
            case 1:
                _spriteRenderer.sharedMaterial.SetColor("_TargetColor1", healPalette.backgroundColor);
                _spriteRenderer.sharedMaterial.SetColor("_TargetColor2", healPalette.foregroundColor);
                break;
            //GOLD
            case 2:
                _spriteRenderer.sharedMaterial.SetColor("_TargetColor1", _goldPalette.backgroundColor);
                _spriteRenderer.sharedMaterial.SetColor("_TargetColor2", _goldPalette.foregroundColor);
                break;
        }
    }

    bool _isDeathEffectActive = false;
    public void DeathEffect(float speed = 4.5f)
    {
        if (!_isDeathEffectActive)
        {
            StartCoroutine(CRT_DeathEffect(speed));
        }
    }

    private IEnumerator CRT_DeathEffect(float speed)
    {
        float t = 0;

        _isDeathEffectActive = true;

        _screenQuad.sharedMaterial.SetFloat("_TimePassed", 0.999f);
        _screenQuad.sharedMaterial.SetFloat("_Alpha", 1);

        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 1f;

        t = 0.999f;
        while(t >= 0)
        {
            _screenQuad.sharedMaterial.SetFloat("_TimePassed", t);

            t -= Time.deltaTime * speed;

            yield return null;
        }

        _screenQuad.sharedMaterial.SetFloat("_TimePassed", 0);
        _screenQuad.sharedMaterial.SetFloat("_Alpha", 0);
        _isDeathEffectActive = false;

        yield break;
    }
}
