using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : BaseState
{
    protected Enemy _enemy;

    public EnemyBaseState(Enemy enemy)
    {
        _enemy = enemy;
    }
}
