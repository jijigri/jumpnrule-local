using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : Pickup
{
    [SerializeField] private int _moneyAmount = 1;

    protected override void OnPlayerTrigger()
    {
        if (_player.TryGetComponent<RunnerStat>(out var runnerStat))
        {
            runnerStat.IncreaseGold(_moneyAmount);

            BackgroundEffectsManager.Instance.GoldEffect(.05f, .1f, .1f);
        }
    }
}
