using UnityEngine;

// Base class for all Items, that is immediately consumed when bought
public abstract class OneTimeItem : Item
{
    public OneTimeItem() : base("OneTimeItem")
    {
    }

    //
    public override void OnAdd(GameObject owner)
    {
        base.OnAdd(owner);
        OnAddOneTime();
        Destroy();
    }

    public abstract void OnAddOneTime();
}
