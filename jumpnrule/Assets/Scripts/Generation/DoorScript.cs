using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] LayerMask _playerMask;
    [SerializeField] private Vector2 _playerDetectionRange = new Vector2(2, 2);
    [SerializeField] private Vector2 _playerDetectionOffset = new Vector2(0, 0);
    [SerializeField] private Sprite _lockedDoorSprite = null;
    [SerializeField] private Sprite _unlockedDoorSprite = null;
    [SerializeField] private SpriteRenderer _outline = null;

    private BoxCollider2D _boxCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private WorldRoom _worldRoom;

    bool _isPlayerInRange;
    bool _isDoorUnlocked = false;

    bool _isDoorCreated = false;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        //_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _outline.enabled = false;

        _spriteRenderer.sprite = _lockedDoorSprite;
    }

    private void Update()
    {
        if (!_isDoorCreated)
        {
            return;
        }

        //TODO: Only check when round is over
        CheckForPlayerInRange();

        if (!_isDoorUnlocked)
        {
            if (_isPlayerInRange)
            {
                //TODO: Make it so the key has to be held for a bit to prevent players from opening a room when trying to pick up a weapon
                if (Input.GetKeyDown(KeyCode.F))
                {
                    UnlockDoor();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!_isDoorCreated)
        {
            _outline.sprite = _lockedDoorSprite;
            _outline.enabled = true;
            return;
        }

        if (_isPlayerInRange)
        {
            if (_outline != null)
            {
                _outline.enabled = true;
            }
        }
        else
        {
            if (_outline != null)
            {
                _outline.enabled = false;
            }
        }

        if (_isDoorUnlocked)
        {
            if (_outline != null)
            {
                _outline.enabled = false;
            }
        }
    }

    public WorldRoom GetParentRoom()
    {
        return _worldRoom;
    }

    void CheckForPlayerInRange()
    {
        Collider2D boxCollisionDetection = Physics2D.OverlapBox((Vector2)transform.position + _playerDetectionOffset, _playerDetectionRange, 0, _playerMask);

        if (boxCollisionDetection)
        {
            if (boxCollisionDetection.CompareTag("Player"))
            {
                _isPlayerInRange = true;
            }
            else
            {
                _isPlayerInRange = false;
            }
        }
        else
        {
            _isPlayerInRange = false;
        }
    }

    public void CreateDoor(WorldRoom worldRoom)
    {
        _worldRoom = worldRoom;

        _spriteRenderer.sprite = _unlockedDoorSprite;

        _isDoorCreated = true;
    }

    public void UnlockDoor(bool isNeighbor = false)
    {
        _animator.SetTrigger("open");
        _boxCollider.enabled = false;
        _isDoorUnlocked = true;

        if (_outline != null)
        {
            _outline.enabled = false;
        }

        if (!isNeighbor)
        {
            _worldRoom.UnlockNeighboringRoom(this);
        }
    }
}
