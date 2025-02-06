using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatBehavior : MonoBehaviour
{
    [SerializeField] private float _maxRetreatTime = 2.5f;
    [Range(0, 100)]
    [SerializeField] private float _chancesToRetreat = 100f;

    private float _numberOfRoundsToReachMaxRetreatTime = 15f;

    private float _currentRetreatTime = 0;

    ICanRetreat _canRetreat;
    NavMeshGeneration _navMeshGeneration;

    private void Awake()
    {
        _canRetreat = GetComponent<ICanRetreat>();
        _navMeshGeneration = GameObject.FindObjectOfType<NavMeshGeneration>();
    }

    private void Start()
    {
        GameEvents.Instance.OnRespawnEvent += OnRespawnEvent;

        _currentRetreatTime = LevelManager.Instance.CurrentRound / (_numberOfRoundsToReachMaxRetreatTime / _maxRetreatTime);
        _currentRetreatTime = Mathf.Clamp(_currentRetreatTime, 0, _maxRetreatTime);
        Debug.Log("Current Retreat Time: " + _currentRetreatTime);
    }

    private void OnDestroy()
    {
        GameEvents.Instance.OnRespawnEvent -= OnRespawnEvent;
    }

    void OnRespawnEvent()
    {
        StartRetreat();
    }

    void StartRetreat()
    {
        float random = Random.Range(0, 100);
        if(random < _chancesToRetreat)
        {
            Vector2 randomPosition = _navMeshGeneration.FreePositions[Random.Range(0, _navMeshGeneration.FreePositions.Count)];
            _canRetreat.StartRetreating(randomPosition);

            Debug.Log("ENEMY RETREATING");

            StartCoroutine(CRT_StopRetreat());
        }
    }

    IEnumerator CRT_StopRetreat()
    {
        yield return new WaitForSeconds(_currentRetreatTime);

        StopRetreat();

        yield break;
    }

    void StopRetreat()
    {
        _canRetreat.StopRetreating();
    }
}
