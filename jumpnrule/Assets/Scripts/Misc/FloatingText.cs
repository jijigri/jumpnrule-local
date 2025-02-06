using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TextType
{
    ENEMY_DAMAGE,
    SELF_HEALING
}

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMesh _textMesh = null;
    [SerializeField] private Color _damageColor = default;
    [SerializeField] private Color _healColor = default;

    public void InitializeText(TextType textType, float damage, Vector2 position, Vector2 hitVelocity, DamageType damageType = DamageType.DIRECT)
    {
        transform.position += ((Vector3)hitVelocity.normalized + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)));
        _textMesh.text = damage.ToString();
        _textMesh.color = GetTextColor(textType);
    }

    Color GetTextColor(TextType textType)
    {
        switch (textType)
        {
            case TextType.ENEMY_DAMAGE:
                return _damageColor;
            case TextType.SELF_HEALING:
                return _healColor;
            default:
                return _damageColor;
        }
    }
}
