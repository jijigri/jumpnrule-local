using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        HandleUpdate();
    }

    // The concept here is that ItemManager will dispatch every relevant call to the items
    // This can potentially be done with events and handlers, but the logic who handles what
    // can be potentially very complex, so I thought it's better to keep it here
    // TODO: Potentially enchance it later

    public void HandleUpdate()
    {
        Dictionary<GameObject, List<Item>> inventory = LevelManager.Instance.GetLevelState().GetItemInventory().GetInventory();
        foreach (var entry in inventory)
        {
            foreach (Item item in entry.Value)
            {
                item.OnUpdate();
            }
        }
    }

    public void HandleBuy(GameObject runner, Item item)
    {
        int cost = GetCost(item);

        bool buySuccess = runner.GetComponent<RunnerStat>().TryBuy(cost);
        if (buySuccess)
        {
            LevelManager.Instance.GetLevelState().GetItemInventory().Add(runner, item);
            item.OnAdd(runner);
            Debug.Log("Item buy succeeded");
        }
        else
        {
            Debug.Log("Item buy failed");
        }
    }

    public void HandleUse(GameObject runner, Item item)
    {
        Item inventoryItem = LevelManager.Instance.GetLevelState().GetItemInventory().GetItem(runner, item);
        inventoryItem.OnUse();
        Debug.Log("Item was used");
    }

    public void HandleDestroy(GameObject runner, Item item)
    {
        Item inventoryItem = LevelManager.Instance.GetLevelState().GetItemInventory().Remove(runner, item);
        inventoryItem.OnDestroy();
        Debug.Log("Item was destroyed");
    }

    // --
    private int GetCost(Item item)
    {
        return 2; // TODO Implement properly, for every item
    }

    public void HandleBuyKickerItem()
    {
        // TODO Implement properly, this is just an example
        GameObject runner = LevelManager.Instance.GetLevelState().GetRunner();
        Item item = new KickerItem();

        HandleBuy(runner, item);
    }
}
