using UnityEngine;

public class GoldBehavior : MonoBehaviour
{
    public int value;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<RunnerStat>(out var runnerStat))
        {
            Debug.Log("Gold Collide: " + value + " gold will be granted to the Runner");
            runnerStat.IncreaseGold(value);
            Destroy(this.gameObject);
        }
    }
}
