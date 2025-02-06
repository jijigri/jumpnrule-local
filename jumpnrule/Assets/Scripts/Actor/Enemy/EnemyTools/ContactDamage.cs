using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private float _damage = 30f;
    [SerializeField] private float _attackCooldown = .5f;
    [SerializeField] private Vector2 _hitboxSize = new Vector2(.75f, 1.75f);
    [SerializeField] private LayerMask _hitMask = default;

    private bool _canAttack = true;

    private void Update()
    {
        if (_canAttack)
        {
            Collider2D hit = Physics2D.OverlapBox(transform.position, _hitboxSize, 0, _hitMask);

            if (hit)
            {
                if (hit.TryGetComponent(out ICanBeDamaged canBeDamaged))
                {
                    canBeDamaged.DealDamage(_damage, gameObject, Vector2.zero, DamageType.DIRECT);

                    _canAttack = false;

                    StopCoroutine(CRT_AttackCooldown());
                    StartCoroutine(CRT_AttackCooldown());

                    Debug.Log("DEALING DAMAGE");
                }
            }
        }
    }

    IEnumerator CRT_AttackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);

        _canAttack = true;

        yield break;
    }
}
