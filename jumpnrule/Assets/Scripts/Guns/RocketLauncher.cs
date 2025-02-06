using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : BasicGun
{
    [SerializeField] private string _mineTag = null;
    [SerializeField] private float _mineCooldown = 1f;
    [SerializeField] private int _maxNumberOfMines = 3;
    [SerializeField] private float _mineDamage = 10;
    [SerializeField] private Vector2 _mineLaunchVelocity = Vector2.zero;

    List<Mine> _currentMines = new List<Mine>();

    float _timeBeforePlacingMine = 0;

    public override void Update()
    {
        base.Update();

        if(_timeBeforePlacingMine > 0)
        {
            _timeBeforePlacingMine -= Time.deltaTime;
        }
    }

    public override void OnSecondaryPressed()
    {
        base.OnSecondaryPressed();
        ShootSecondary();
    }

    public override void ShootSecondary()
    {
        if (_timeBeforePlacingMine <= 0)
        {
            base.ShootSecondary();
            SetMine();

            _timeBeforePlacingMine = _mineCooldown;
        }
    }

    void SetMine()
    {
        if (_currentMines.Count >= _maxNumberOfMines)
        {
            _currentMines[0].ExplodeMine();
        }

        GameObject mineObject = _pooler.SpawnObject(_mineTag, _shootPoint.position, Quaternion.identity);
        Vector2 throwDirection = _camera.ScreenToWorldPoint(Input.mousePosition) - _player.transform.position;

        if (throwDirection.y < 5f) throwDirection.y = 5f;

        float clampMagnitude = Mathf.Clamp(throwDirection.magnitude, 1, 10);
        Debug.Log("MAGNITUDE: " + clampMagnitude);
        Vector2 throwVelocity = throwDirection.normalized * clampMagnitude;

        Mine mine = mineObject.GetComponent<Mine>();
        mine.InitializeMine(throwVelocity, _mineDamage, _mineLaunchVelocity, this);
        mine.SetPlayerOwner(_player);
        mine.SetGunOwner(this);

        if(mineObject.TryGetComponent(out PlayerOwnedObject playerOwnedObject))
        {
            playerOwnedObject.SetOwner(_player);
        }

        _currentMines.Add(mine);
    }

    public void RemoveMine(Mine mine)
    {
        if (_currentMines.Contains(mine))
        {
            _currentMines.Remove(mine);
        }
    }
}
