using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameEvents.Instance.onEnemyDamaged += OnEnemyDamaged;
        GameEvents.Instance.onEnemyKilled += OnEnemyKilled;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onEnemyDamaged -= OnEnemyDamaged;
        GameEvents.Instance.onEnemyKilled -= OnEnemyKilled;
    }

    void OnEnemyDamaged(Enemy enemy, GameObject killer)
    {
        _animator.SetTrigger("damage");
    }

    void OnEnemyKilled(Enemy enemy, GameObject killer)
    {
        _animator.SetTrigger("kill");
    }
}
