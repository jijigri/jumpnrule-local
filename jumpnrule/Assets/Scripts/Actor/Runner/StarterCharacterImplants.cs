using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterCharacterImplants : PlayerImplants
{
    ObjectPooler _pooler;

    public override void Start()
    {
        base.Start();
        GameEvents.Instance.onEnemyKilled += OnEnemyKilled;

        _pooler = ObjectPooler.Instance;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameEvents.Instance.onEnemyKilled -= OnEnemyKilled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            HudManager.Instance.SetImplantPanel(true);
            ImplantsManager.Instance.FillPanel();
        }
    }

    private void OnEnemyKilled(Enemy enemy, GameObject killer)
    {
        if (killer != null)
        {
            Debug.Log("STARTCHARACTERIMPLANTS: " + killer.gameObject.name);

            if (_currentImplants.Contains(0))
            {
                if (killer == gameObject)
                {
                    for (int i = 0; i < enemy.tier + 1; i++)
                    {
                        _pooler.SpawnObject("HealthOrb", enemy.transform.position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
                    }
                }
            }
        }
    }
}
