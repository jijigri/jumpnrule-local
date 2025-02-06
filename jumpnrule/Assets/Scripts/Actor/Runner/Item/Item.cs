using UnityEngine;

// Abstract base class for all Items
public abstract class Item
{
    private string _tag;
    protected GameObject _owner; // TODO Nullable?

    public Item(string tag = "Item")
    {
        _tag = tag;
    }
    
    public string GetTag()
    {
        return _tag;
    }

    //
    public virtual void OnAdd(GameObject owner)
    {
        _owner = owner;
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnUse()
    {
    }

    public virtual void OnDestroy()
    {
    }

    public void Destroy()
    {
        if (_owner == null)
        {
            throw new System.Exception("Destroy(): Item has no owner");
        }
        ItemManager.Instance.HandleDestroy(_owner, this);
    }
}
