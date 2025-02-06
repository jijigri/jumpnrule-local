using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

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

    public event Action<int> onRoundCleared;
    public void WaveCleared(int waveIndex)
    {
        if (onRoundCleared != null)
        {
            onRoundCleared(waveIndex);
        }
    }

    public event Action<int> onRoundStarted;
    public void WaveStart(int waveIndex)
    {
        if (onRoundStarted != null)
        {
            onRoundStarted(waveIndex);
        }
    }

    public event Action onEnemiesSpawned;
    public void EnemiesSpawned()
    {
        if(onEnemiesSpawned != null)
        {
            onEnemiesSpawned();
        }
    }

    public event Action<int, int> onUpgradeChosen;
    public void UpdateChosen(int id, int level)
    {
        if (onUpgradeChosen != null)
        {
            onUpgradeChosen(id, level);
        }
    }

    public event Action<Enemy, GameObject> onEnemyDamaged;
    public void EnemyDamaged(Enemy enemy, GameObject killer)
    {
        if (onEnemyDamaged != null)
        {
            onEnemyDamaged(enemy, killer);
        }
    }

    public event Action<Enemy, GameObject> onEnemyKilled;
    public void EnemyKilled(Enemy enemy, GameObject killer)
    {
        if(onEnemyKilled != null)
        {
            onEnemyKilled(enemy, killer);
        }
    }

    public event Action<PlatformController> onPlayerRoll;
    public void PlayerRoll(PlatformController controller)
    {
        if(onPlayerRoll != null)
        {
            onPlayerRoll(controller);
        }
    }

    public event Action<float, PlayerHealthManager> onPlayerDamaged;
    public void PlayerDamaged(float damageAmount, PlayerHealthManager playerHealthManager)
    {
        if(onPlayerDamaged != null)
        {
            onPlayerDamaged(damageAmount, playerHealthManager);
        }
    }

    public event Action<float, PlayerHealthManager> onPlayerHealed;
    public void PlayerHealed(float healAmount, PlayerHealthManager playerHealthManager)
    {
        if (onPlayerHealed != null)
        {
            onPlayerHealed(healAmount, playerHealthManager);
        }
    }

    public event Action<float, PlayerHealthManager> onPlayerGetArmor;
    public void PlayerGetArmor(float armorAmount, PlayerHealthManager playerHealthManager)
    {
        if(onPlayerGetArmor != null)
        {
            onPlayerGetArmor(armorAmount, playerHealthManager);
        }
    }

    public event Action<float, PlayerAbilityManager> onPlayerGetEnergy;
    public void PlayerGetEnergy(float energyAmount, PlayerAbilityManager playerAbilityManager)
    {
        if (onPlayerGetEnergy != null)
        {
            onPlayerGetEnergy(energyAmount, playerAbilityManager);
        }
    }

    public event Action<float, PlayerAbilityManager> onPlayerConsumeEnergy;
    public void PlayerConsumeEnergy(float energyAmount, PlayerAbilityManager playerAbilityManager)
    {
        if (onPlayerConsumeEnergy != null)
        {
            onPlayerConsumeEnergy(energyAmount, playerAbilityManager);
        }
    }

    public event Action<PlayerHealthManager> onPlayerDeath;
    public void PlayerDeath(PlayerHealthManager playerHealthManager)
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath(playerHealthManager);
        }
    }

    public event Action<Gun> onEnemyFalter;
    public void EnemyFalter(Gun gun)
    {
        if(onEnemyFalter != null)
        {
            onEnemyFalter(gun);
        }
    }

    public event Action<float> OnRespawnEventTick;
    public void RespawnEventTick(float value)
    {
        if(OnRespawnEventTick != null)
        {
            OnRespawnEventTick(value);
        }
    }

    public event Action OnRespawnEvent;
    public void RespawnEvent()
    {
        if(OnRespawnEvent != null)
        {
            OnRespawnEvent();
        }
    }
}
