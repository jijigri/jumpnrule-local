using System.Collections.Generic;
using UnityEngine;

public class ItemInventory
{
    private Dictionary<GameObject, List<Item>> _inventory;
    
    public ItemInventory(List<GameObject> runners)
    {
        _inventory = new Dictionary<GameObject, List<Item>>();

        foreach(GameObject runner in runners)
        {
            _inventory.Add(runner, new List<Item>());
        }
    }

    private List<Item> getRunnerInventory(GameObject runner)
    {
        if (!_inventory.ContainsKey(runner))
        {
            throw new System.Exception("No inventory is assigned to Runner");
        }

        return _inventory[runner];
    }

    public void Clear()
    {
        foreach(var entry in _inventory)
        {
            entry.Value.Clear();
        }
    }

    public void Add(GameObject runner, Item item)
    {
        List<Item> items = getRunnerInventory(runner);

        items.Add(item);
    }

    // Removes `item` from `runner`
    public Item Remove(GameObject runner, Item item)
    {
        Item itemToBeRemoved;
        List<Item> items = getRunnerInventory(runner);

        int index = items.FindIndex(i => i == item); // Here we need == instead of Equals, as we want to remove the actual item
        if (index >= 0)
        {
            itemToBeRemoved = items[index];
            items.RemoveAt(index);
            return itemToBeRemoved;
        }

        throw new System.Exception("ItemInventory.Remove(item): No such item is assigned to Runner");
    }

    // Removes the first item from `runner` that has `tag` tag
    public Item Remove(GameObject runner, string tag)
    {
        Item itemToBeRemoved;
        List<Item> items = getRunnerInventory(runner);

        int index = items.FindIndex(i => i.GetTag() == tag);
        if (index >= 0)
        {
            itemToBeRemoved = items[index];
            items.RemoveAt(index);
            return itemToBeRemoved;
        }

        throw new System.Exception("ItemInventory.Remove(tag): No such item is assigned to Runner");
    }

    public bool HasItemWithTag(GameObject runner, string tag)
    {
        List<Item> items = getRunnerInventory(runner);

        foreach (var item in items)
        {
            if (item.GetTag() == tag)
            {
                return true;
            }
        }
        return false;
    }
    public Item GetItem(GameObject runner, Item item)
    {
        List<Item> items = getRunnerInventory(runner);

        int index = items.FindIndex(i => i == item);
        if (index >= 0)
        {
            return items[index];
        }

        throw new System.Exception("ItemInventory.GetItem(item): No such item is assigned to Runner");
    }

    public Item GetItem(GameObject runner, string tag)
    {
        List<Item> items = getRunnerInventory(runner);

        int index = items.FindIndex(i => i.GetTag() == tag);
        if (index >= 0)
        {
            return items[index];
        }

        throw new System.Exception("ItemInventory.GetItem(tag): No such item is assigned to Runner");
    }

    public Dictionary<GameObject, List<Item>> GetInventory()
    {
        return _inventory;
    }
}
