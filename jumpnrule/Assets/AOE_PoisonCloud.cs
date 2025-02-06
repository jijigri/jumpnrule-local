using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE_PoisonCloud : AreaOfEffect
{
    [SerializeField] private float _damage;
    [SerializeField] private float _timeBetweenTicks;
    [SerializeField] private ParticleSystem _bubbles = null;
    [SerializeField] private ParticleSystem _zone = null;
    [SerializeField] private ParticleSystem _zoneRadius = null;
    [SerializeField] private ParticleSystem _clouds = null;
    [SerializeField] private CircleCollider2D _circleCollider = null;

    List<Transform> _targets = new List<Transform>();

    float _bubbleStartSize;
    float _zoneStartSize;
    float _radiusStartSize;
    float _cloudStartSize;
    float _colliderStartSize;

    StatusEffectBehavior poisonData;

    private void Awake()
    {
        _bubbleStartSize = _bubbles.shape.radius;
        _zoneStartSize = _zone.shape.radius;
        _radiusStartSize = _zoneRadius.shape.radius;
        _cloudStartSize = _clouds.shape.radius;
        _colliderStartSize = _circleCollider.radius;
    }

    private void Start()
    {
        InitializeAOE(_radius);
    }

    public void InitializeAOE(float radius, float damage, float timeBetweenTicks)
    {
        base.InitializeAOE(radius);

        var bubbleShape = _bubbles.shape;
        bubbleShape.radius = _bubbleStartSize * _radius;

        _zone.startSize = _zoneStartSize * _radius;

        var radiusShape = _zoneRadius.shape;
        radiusShape.radius = _radiusStartSize * _radius;

        var cloudShape = _clouds.shape;
        cloudShape.radius = _cloudStartSize * _radius;

        _circleCollider.radius = _colliderStartSize * radius;

        poisonData = StatusEffectsPool.Instance.poisonStatusEffect;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(poisonData == null)
        {
            poisonData = StatusEffectsPool.Instance.poisonStatusEffect;
        }

        if (!_targets.Contains(collision.transform))
        {
            if(collision.transform.TryGetComponent(out StatusEffectsManager statusEffectsManager))
            {
                statusEffectsManager.SetStatusEffect(poisonData, 500);
            }

            _targets.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_targets.Contains(collision.transform))
        {
            if (collision.transform.TryGetComponent(out StatusEffectsManager statusEffectsManager))
            {
                statusEffectsManager.RemoveStatusEffect(poisonData);
            }

            _targets.Remove(collision.transform);
        }
    }
}