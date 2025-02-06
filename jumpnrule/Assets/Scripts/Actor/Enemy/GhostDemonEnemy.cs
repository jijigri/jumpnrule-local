using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDemonEnemy : Enemy
{
    [SerializeField] private float _playerDataUpdateTickTime = .2f;
    [SerializeField] private float _initialMoveSpeed = 50f;
    
    private float _currentMoveSpeed = 50f;

    private Transform _target;
    private Queue<PlayerData> _playerData = new Queue<PlayerData>();
    private int _maxNumberOfData = 25;

    bool _isRecording = true;
    bool _isChasing = true;

    PlayerData _currentData;

    SpriteRenderer _targetSpriteRenderer;
    Transform _targetGraphics;

    protected override void Start()
    {
        base.Start();

        _currentMoveSpeed = _initialMoveSpeed + (LevelManager.Instance.CurrentRound * 2f);
        gameObject.name = gameObject.name + " " + _currentMoveSpeed;

        _target = _player.transform;
        _targetSpriteRenderer = _target.GetComponent<RunnerStat>().spriteRenderer;
        _targetGraphics = _target.GetComponent<RunnerStat>().graphics;
        StartCoroutine(StartRecordingPlayerData());
    }

    private IEnumerator StartRecordingPlayerData()
    {
        _currentData = new PlayerData(_rigidBody.position, _targetSpriteRenderer.sprite, (int)_targetGraphics.localScale.x);

        /*
        GhostDemonEnemy[] existingGhosts = FindObjectsOfType<GhostDemonEnemy>();

        float timeOffset = 0;
        if (existingGhosts != null)
        {
            for (int i = 0; i < existingGhosts.Length; i++)
            {
                timeOffset += .25f;
            }
        }
        */

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        while (_isRecording)
        {
            if(_playerData.Count >= _maxNumberOfData)
            {
                _playerData.Dequeue();
                Debug.LogWarning("Dequeuing a position because Queue was too long");
            }

            if (_playerData.Count < _maxNumberOfData)
            {
                _playerData.Enqueue(new PlayerData(_target.position, _targetSpriteRenderer.sprite, (int)_targetGraphics.localScale.x));
            }
            else
            {
                Debug.LogError("GhostDemonEnemy.cs: Max queue size reached for _playerData");
            }

            if (_playerDataUpdateTickTime > 0)
            {
                yield return new WaitForSeconds(_playerDataUpdateTickTime);
            }
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (_isChasing)
        {
            if (Vector2.Distance(_rigidBody.position, _currentData.position) < .6f)
            {
                if (_playerData.Count > 1)
                {
                    _currentData = _playerData.Dequeue();
                }
                else
                {
                    //Debug.LogWarning("GhostDemonEnemy.cs: Ghost was too fast for its Update time");
                }
            }
            else
            {
                Vector2 direction = (_currentData.position - _rigidBody.position).normalized;
                _rigidBody.AddForce(direction * (_currentMoveSpeed * 100) * Time.fixedDeltaTime);
            }

            _spriteRenderer.sprite = _currentData.sprite;
            _spriteRenderer.flipX = _currentData.faceDirection == -1 ? true : false;
        }
    }

    protected override void OnFalterStart()
    {
        base.OnFalterStart();

        _isChasing = false;
    }

    protected override void OnFalterStop()
    {
        base.OnFalterStop();

        _isChasing = true;
    }

    public struct PlayerData
    {
        public Vector2 position;
        public Sprite sprite;
        public int faceDirection;

        public PlayerData(Vector2 position, Sprite sprite, int faceDirection)
        {
            this.position = position;
            this.sprite = sprite;
            this.faceDirection = faceDirection;
        }
    }
}
