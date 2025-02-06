using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(BoxCollider2D))]
public class ColorRegion : MonoBehaviour
{
    [SerializeField] private ColorData _color;
    [SerializeField] private float _fogStrength = .15f;

    private Light2D _regionLight;

    Vector2 _boxOffset;
    Vector2 _boxSize;

    BoxCollider2D _boxCollider;

    private void Awake()
    {
        UpdateRegionLight();
        _boxCollider.enabled = false;
    }

    void Start()
    {
        Light2D[] lights = FindObjectsOfType<Light2D>();

        foreach(Light2D light in lights)
        {
            if (light.lightType != Light2D.LightType.Global)
            {
                if (CheckIfLightInRange(light.transform))
                {
                    light.color = _color.color;
                }
            }
        }

    }

    [ContextMenu("Update Region Light")]
    private void UpdateRegionLight()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxOffset = _boxCollider.offset;
        _boxSize = _boxCollider.size;

        _regionLight = GetComponentInChildren<Light2D>();

        _regionLight.transform.position = transform.position + (Vector3)_boxOffset;
        _regionLight.transform.localScale = new Vector3(_boxSize.x, _boxSize.y, 1);

        _regionLight.color = _color.color;
        _regionLight.intensity = _fogStrength;
    }

    bool CheckIfLightInRange(Transform light)
    {
        bool isLightInRange = false;

        Vector2 position = (Vector2)transform.position + _boxOffset;
        Vector2 lightPosition = light.position;

        if(lightPosition.x > transform.position.x - (_boxSize.x / 2) && lightPosition.x < transform.position.x + (_boxSize.x / 2))
        {
            if (lightPosition.y > transform.position.y - (_boxSize.y / 2) && lightPosition.y < transform.position.y + (_boxSize.y / 2))
            {
                isLightInRange = true;
            }
        }

        return isLightInRange;
    }
}
