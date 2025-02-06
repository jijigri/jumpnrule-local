using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private List<GameObject> _demons = new List<GameObject>();
    [SerializeField] private List<GameObject> _creatures = new List<GameObject>();

    private List<GameObject> _energyCreatures = new List<GameObject>();
    private List<GameObject> _healthCreatures = new List<GameObject>();
    private List<GameObject> _armorCreatures = new List<GameObject>();
    private List<GameObject> _speedCreatures = new List<GameObject>();

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

        foreach(GameObject obj in _creatures)
        {
            if(obj.TryGetComponent(out Enemy enemy))
            {
                switch (enemy.enemyType)
                {
                    case EnemyType.ENERGY_CREATURE:
                        _energyCreatures.Add(obj);
                        break;
                    case EnemyType.HEALTH_CREATURE:
                        _healthCreatures.Add(obj);
                        break;
                    case EnemyType.ARMOR_CREATURE:
                        _armorCreatures.Add(obj);
                        break;
                    case EnemyType.SPEED_CREATURE:
                        _speedCreatures.Add(obj);
                        break;
                }
            }
        }
    }

    public GameObject GetRandomEnemy(EnemyType enemyType, int tier)
    {
        GameObject enemy = _demons[0];

        switch(enemyType)
        {
            case EnemyType.DEMON:
                enemy = _demons[Random.Range(0, _demons.Count)];
                break;

            case EnemyType.ENERGY_CREATURE:
                enemy = _energyCreatures[Random.Range(0, _energyCreatures.Count)];
                break;

            case EnemyType.HEALTH_CREATURE:
                enemy = _healthCreatures[Random.Range(0, _healthCreatures.Count)];
                break;

            case EnemyType.ARMOR_CREATURE:
                enemy = _armorCreatures[Random.Range(0, _armorCreatures.Count)];
                break;

            case EnemyType.SPEED_CREATURE:
                enemy = _speedCreatures[Random.Range(0, _speedCreatures.Count)];
                break;
        }
        

        return enemy;
    }

    /*
    public List<GameObject> GetEnemiesToSpawn(Vector2Int numberOfEnemiesToSpawn)
    {
        List<GameObject> enemiesToSpawn = new List<GameObject>();

        for(int x = 0; x < numberOfEnemiesToSpawn.x; x++)
        {
            int attemptIndex = 0;
            while(true)
            {
                Enemy randomEnemy = enemies[Random.Range(0, enemies.Count)].GetComponent<Enemy>();
                if(randomEnemy.canRespawn == false)
                {
                    enemiesToSpawn.Add(randomEnemy.gameObject);
                    break;
                }

                attemptIndex++;
                if(attemptIndex >= 5000)
                {
                    Debug.LogError("EnemyPool: Couldn't find suitable enemy (Chaser)");
                    break;
                }
            }
        }

        for (int y = 0; y < numberOfEnemiesToSpawn.y; y++)
        {
            int attemptIndex = 0;
            while (true)
            {
                Enemy randomEnemy = enemies[Random.Range(0, enemies.Count)].GetComponent<Enemy>();
                if (randomEnemy.canRespawn == true)
                {
                    enemiesToSpawn.Add(randomEnemy.gameObject);
                    break;
                }

                attemptIndex++;
                if (attemptIndex >= 5000)
                {
                    Debug.LogError("EnemyPool: Couldn't find suitable enemy (Target)");
                    break;
                }
            }
        }

        return enemiesToSpawn;
    }
    */
}
