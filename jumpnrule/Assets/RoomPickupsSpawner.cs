using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPickupsSpawner : MonoBehaviour
{
    private Tilemap _tilemap;

    List<Vector2> _healthPickupSpawnPoints = new List<Vector2>();
    List<Vector2> _armorPickupSpawnPoints = new List<Vector2>();
    List<Vector2> _energyPickupSpawnPoints = new List<Vector2>();

    List<GameObject> _spawnedPickups = new List<GameObject>();

    private void Awake()
    {
        _tilemap = transform.Find("Grid").Find("PickupsTilemap").GetComponent<Tilemap>();

        if (_tilemap != null)
        {
            for (int x = _tilemap.cellBounds.xMin; x < _tilemap.cellBounds.xMax; x++)
            {
                for (int y = _tilemap.cellBounds.yMin; y < _tilemap.cellBounds.yMax; y++)
                {
                    Vector3Int localPlace = (new Vector3Int(x, y, (int)_tilemap.transform.position.y));
                    Vector3 place = _tilemap.CellToWorld(localPlace);
                    if (_tilemap.HasTile(localPlace))
                    {
                        TileBase tile = _tilemap.GetTile(localPlace);

                        if (tile.name == "HealthPickupTile")
                        {
                            _healthPickupSpawnPoints.Add(place);
                        }
                        else if (tile.name == "ArmorPickupTile")
                        {
                            _armorPickupSpawnPoints.Add(place);
                        }
                        else if (tile.name == "EnergyPickupTile")
                        {
                            _energyPickupSpawnPoints.Add(place);
                        }
                    }
                }
            }

            _tilemap.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("OOPS");
        }
    }

    private void Start()
    {
        GameEvents.Instance.onRoundStarted += OnRoundStarted;
        GameEvents.Instance.onRoundCleared += OnRoundCleared;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onRoundStarted -= OnRoundStarted;
        GameEvents.Instance.onRoundCleared -= OnRoundCleared;
    }

    void OnRoundStarted(int roundIndex)
    {
        foreach(Vector2 spawnPoint in _healthPickupSpawnPoints)
        {
            GameObject pickup = ObjectPooler.Instance.SpawnObject("SpeedPickup", spawnPoint, Quaternion.identity);
            _spawnedPickups.Add(pickup);
        }

        foreach (Vector2 spawnPoint in _armorPickupSpawnPoints)
        {
            GameObject pickup = ObjectPooler.Instance.SpawnObject("ArmorPickup", spawnPoint, Quaternion.identity);
            _spawnedPickups.Add(pickup);
        }

        foreach (Vector2 spawnPoint in _energyPickupSpawnPoints)
        {
            GameObject pickup = ObjectPooler.Instance.SpawnObject("EnergyPickup", spawnPoint, Quaternion.identity);
            _spawnedPickups.Add(pickup);
        }
    }

    void OnRoundCleared(int roundIndex)
    {
        foreach(GameObject pickup in _spawnedPickups)
        {
            if(pickup != null)
            {
                pickup.gameObject.SetActive(false);
            }
        }

        _spawnedPickups.Clear();
    }
}
