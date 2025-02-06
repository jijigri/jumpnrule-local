using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackType
{
    MELEE,
    RANGED
}

public class EnemyAttacksCooldownManager : MonoBehaviour
{
    public static EnemyAttacksCooldownManager Instance { get; private set; }

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

    //INCREASE OVER WAVES TO MAKE IT HARDER
    [SerializeField] private float _meleeAttacksCooldown = .25f;
    [SerializeField] private int _meleeAttacksQueueLimit = 5;
    [SerializeField] private float _rangedAttacksCooldown = .25f;
    [SerializeField] private int _rangedAttacksQueueLimit = 5;

    Queue<Action> _meleeAttacksQueue = new Queue<Action>();
    Queue<Action> _rangedAttacksQueue = new Queue<Action>();

    float _currentMeleeTimer = 0;
    float _currentRangedTimer = 0;

    public void RequestAttack(EnemyAttackType attackType, Action callback)
    {
        switch (attackType)
        {
            case EnemyAttackType.MELEE:

                if(_meleeAttacksQueue.Count >= _meleeAttacksQueueLimit)
                {
                    Debug.Log("Skipped cooldown because queue was full");
                    Action action = _meleeAttacksQueue.Dequeue();
                    if (action.Method != null)
                    {
                        action?.Invoke();
                    }
                }

                _meleeAttacksQueue.Enqueue(callback);
                break;
            case EnemyAttackType.RANGED:

                if (_rangedAttacksQueue.Count >= _rangedAttacksQueueLimit)
                {
                    Debug.Log("Skipped cooldown because queue was full");
                    Action action = _meleeAttacksQueue.Dequeue();
                    if (action.Method != null)
                    {
                        action?.Invoke();
                    }
                }

                _rangedAttacksQueue.Enqueue(callback);
                break;
        }
    }

    private void Update()
    {
        if(_currentMeleeTimer <= 0)
        {
            if(_meleeAttacksQueue.Count > 0)
            {
                Action action = _meleeAttacksQueue.Dequeue();
                if (action.Method != null)
                {
                    action?.Invoke();
                }

                _currentMeleeTimer = _meleeAttacksCooldown;
            }
        }
        else
        {
            _currentMeleeTimer -= Time.deltaTime;
        }

        if (_currentRangedTimer <= 0)
        {
            if (_rangedAttacksQueue.Count > 0)
            {
                Action action = _rangedAttacksQueue.Dequeue();
                if (action.Method != null)
                {
                    action?.Invoke();
                }


                _currentRangedTimer = _rangedAttacksCooldown;
            }
        }
        else
        {
            _currentRangedTimer -= Time.deltaTime;
        }
    }
}
