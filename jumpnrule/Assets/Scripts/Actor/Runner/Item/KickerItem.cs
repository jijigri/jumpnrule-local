using UnityEngine;

// An Item, that kicks you with a force when you buy it
public class KickerItem : OneTimeItem
{
    public KickerItem() : base()
    {
    }

    public override void OnAddOneTime()
    {
        RunnerControl control = LevelManager.Instance.GetLevelState().GetRunner().GetComponent<RunnerControl>(); // TODO Extend for more Runners
        control.AddImpulse(new Vector2(0, 40));
    }
}
