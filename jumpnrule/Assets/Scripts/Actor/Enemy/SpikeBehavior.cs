using UnityEngine;

public class SpikeBehavior : MonoBehaviour
{
    public double damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Spike Collide: collision tag = " + collision.gameObject.tag);

        if (collision.gameObject.TryGetComponent<RunnerStat>(out var runnerStat))
        {
            Debug.Log("Spike Collide: " + damage + " damage will be dealt to the Runner");
            runnerStat.DecreaseHealth(damage);
        }
    }
}
