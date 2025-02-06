using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LineNeon : MonoBehaviour
{
    LineRenderer _lineRenderer = null;
    Light2D _light;
    Transform _pointA = null;
    Transform _pointB = null;

    private void Awake()
    {
        SetComponents();
    }

    void SetComponents()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _light = GetComponentInChildren<Light2D>();
        _pointA = transform.Find("A");
        _pointB = transform.Find("B");
    }

    private void OnValidate()
    {
        UpdateNeon();
    }

    [ContextMenu("Reset Neon")]
    void ResetNeon()
    {
        _lineRenderer.SetPosition(0, Vector2.zero);
        _lineRenderer.SetPosition(1, Vector2.zero);

        UpdateNeon();
    }

    [ContextMenu("Update Neon")]
    void UpdateNeon()
    {
        SetComponents();

        Vector3 direction = (_pointB.localPosition - _pointA.localPosition).normalized;
        Vector3 perpendicular = Vector2.Perpendicular(direction);

        _lineRenderer.SetPosition(0, _pointA.localPosition);
        _lineRenderer.SetPosition(1, _pointB.localPosition);

        Vector3 aPosTop = _pointA.localPosition + (perpendicular * .05f);
        Vector3 aPosBot = _pointA.localPosition - (perpendicular * .05f);

        Vector3 bPosTop = _pointB.localPosition + (perpendicular * .05f);
        Vector3 bPosBot = _pointB.localPosition - (perpendicular * .05f);
        _light.shapePath.SetValue(aPosTop, 0);
        _light.shapePath.SetValue(aPosBot, 1);
        _light.shapePath.SetValue(bPosBot, 2);
        _light.shapePath.SetValue(bPosTop, 3);
    }
}
