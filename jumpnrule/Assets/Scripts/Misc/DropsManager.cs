using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public string dropObjectId;
    public int dropAmount;
    [Range(0, 100)]
    public int dropChance = 100;
}

[System.Serializable]
public class DropPool
{
    public List<ItemDrop> drops = new List<ItemDrop>();
}

public class DropsManager : MonoBehaviour
{
    [SerializeField] private DropPool[] _dropPools = new DropPool[1];

    ObjectPooler _pooler;

    private void Start()
    {
        _pooler = ObjectPooler.Instance;
    }

    public void Drop(int poolIndex = 0)
    {
        if (_dropPools.Length <= poolIndex) return;

        DropPool pool = _dropPools[poolIndex];
        if(pool != null)
        {
            DropItemsFromPool(pool);
        }
    }

    private void DropItemsFromPool(DropPool pool)
    {
        if (pool != null)
        {
            foreach (ItemDrop drop in pool.drops)
            {
                for (int i = 0; i < drop.dropAmount; i++)
                {
                    int chanceToDrop = Random.Range(0, 100);
                    if (chanceToDrop < drop.dropChance)
                    {
                        _pooler.SpawnObject(drop.dropObjectId, transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
}
