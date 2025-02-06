using UnityEngine;

public class RunnerStat : MonoBehaviour
{
    [SerializeField] private double _health; // [0.0, 1.0]
    [SerializeField] private int _gold; // [0, inf)

    private static double MAX_HEALTH = 1.0;

    public SpriteRenderer spriteRenderer;
    public Transform graphics;

    public void StartLevel()
    {
        // TODO Handle reset better
        _health = 1.0;
        _gold = 0;
    }

    public double GetHealth()
    {
        return _health;
    }

    public bool IsDead()
    {
        return _health <= 0.0;
    }

    public void Kill()
    {
        _health = 0.0;
    }

    public void RestoreHealth()
    {
        _health = MAX_HEALTH;
    }

    public void IncreaseHealth(double value)
    {
        if (!IsDead())
        {
            _health += value;
        }

        _health = System.Math.Min(_health, MAX_HEALTH);
    }

    public void DecreaseHealth(double value)
    {
        if (value > _health)
        {
            Kill();
        }
        else
        {
            _health -= value;
        }
    }

    public int GetGold()
    {
        return _gold;
    }

    public bool CanBuy(int cost)
    {
        return cost <= _gold;
    }

    public bool TryBuy(int cost)
    {
        if (CanBuy(cost))
        {
            DecreaseGold(cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void IncreaseGold(int value)
    {
        _gold += value;
    }

    public void DecreaseGold(int value)
    {
        if (value > _gold)
        {
            throw new System.Exception("Runner has " + _gold + " gold, but it was decreased by " + value);
        }
        else
        {
            _gold -= value;
        }
    }

    public override string ToString()
    {
        return "Health = " + _health
            + ", Gold = " + _gold;
    }
}
