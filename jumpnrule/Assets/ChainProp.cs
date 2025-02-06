using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainProp : MonoBehaviour
{
    [SerializeField] GameObject _ceilingLamp = null;
    [SerializeField] private bool _hasLight = false;
    [SerializeField] private bool _isHead = false;
    [SerializeField] private int _minSize = 1;
    [SerializeField] private int _maxSize = 10;

    List<Transform> _children = new List<Transform>();

    Rigidbody2D _rigidBody = null;

    private void Awake()
    {
        if (!_isHead)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        foreach(Transform tr in transform)
        {
            _children.Add(tr);
        }
    }

    private void Start()
    {
        _maxSize = Mathf.Clamp(_maxSize, 2, 10);
        _minSize = Mathf.Clamp(_minSize, 1, _maxSize);

        int randomSize = Random.Range(_minSize, _maxSize);

        for(int i = 1; i < _children.Count; i++)
        {
            if(i == randomSize)
            {
                GameObject chain = _children[i].gameObject;
                Instantiate(_ceilingLamp, chain.transform.position, Quaternion.identity, chain.transform);
                chain.GetComponent<SpriteRenderer>().enabled = false;
            }
            else if(i > randomSize)
            {
                _children[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isHead)
        {
            Vector2 direction = (Vector2)collision.transform.position - (Vector2)transform.position;
            direction.Normalize();

            _rigidBody.AddForce(direction * -5, ForceMode2D.Impulse);
        }
    }
}
