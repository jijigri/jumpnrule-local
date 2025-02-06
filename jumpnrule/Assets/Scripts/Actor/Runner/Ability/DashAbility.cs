public class DashAbility : Ability
{
    public float _maxCooldown;
    public float _strength;

    protected override float GetMaxCooldown()
    {
        return _maxCooldown;
    }

    protected override void Cast()
    {
        this.gameObject.GetComponentInParent<RunnerControl>().AddImpulse(_strength);
    }
}
