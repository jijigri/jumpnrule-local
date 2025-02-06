using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private GameObject enemyHolder;
    [SerializeField] private GameObject chaserPrefab;
    [SerializeField] private int chaserCount;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        var chasers = enemyHolder.GetComponentsInChildren<ChaserBehavior>();
        if (chasers.Length < chaserCount)
        {
            // TODO: Be more sophisticated
            Vector2 playerPosition = LevelManager.Instance.GetLevelState().GetRunner().transform.position;
            Vector2 position;
            do
            {
                position = new Vector2(Random.Range(-15.0f, 15.0f), Random.Range(-15.0f, 15.0f));
            } while ((playerPosition - position).magnitude < 15.0);
            
            GameObject chaser = Instantiate(chaserPrefab, position, Quaternion.identity, enemyHolder.transform);
        }
    }

}
