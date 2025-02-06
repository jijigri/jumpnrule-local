using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int _numberOfRoomsToGenerate = 25;
    [SerializeField] GameObject _defaultRoom = null;
    [SerializeField] LayerMask _roomMask = default;

    RoomPooler _roomPooler;

    Dictionary<Vector2, WorldRoom> _occupiedPositions = new Dictionary<Vector2, WorldRoom>();

    private Vector2 _normalRoomSize = new Vector2(42, 24);

    private void Awake()
    {
        _roomPooler = GetComponent<RoomPooler>();
    }

    private void Start()
    {
        InitializeDefaultRoom();
        StartGeneration();
    }

    void InitializeDefaultRoom()
    {
        WorldRoom defaultRoomScript = _defaultRoom.GetComponent<WorldRoom>();
        _occupiedPositions.Add(_defaultRoom.transform.position, defaultRoomScript);
        defaultRoomScript.UnlockRoom();
    }

    void StartGeneration()
    {
        for (int i = 0; i < _numberOfRoomsToGenerate; i++)
        {
            GameObject generatedRoom = null;

            int error = 0;
            while (generatedRoom == null)
            {
                GameObject randomRoomToGenerateObject = _roomPooler.GetRandomRoom();
                WorldRoom randomRoomToGenerateScript = randomRoomToGenerateObject.GetComponent<WorldRoom>();

                //Tries really hard to find a place for any room selected, before selecting a new random room after 1500 attempts
                for (int attempt = 0; attempt < 1500; attempt++)
                {
                    int randomIndex = Random.Range(0, _occupiedPositions.Count);
                    Vector2 referenceRoomPosition = _occupiedPositions.ElementAt(randomIndex).Key;
                    WorldRoom referenceRoomScript = _occupiedPositions.ElementAt(randomIndex).Value;

                    Vector2 direction = GetRandomDirection();

                    bool collisionTest = Physics2D.OverlapBox(referenceRoomPosition + (direction * _normalRoomSize * referenceRoomScript.roomSizeMultiplier), randomRoomToGenerateScript.roomSizeMultiplier * _normalRoomSize, 0, _roomMask);

                    if (!collisionTest)
                    {
                        if (!_occupiedPositions.ContainsKey(referenceRoomPosition + (direction * _normalRoomSize * referenceRoomScript.roomSizeMultiplier)))
                        {
                            generatedRoom = Instantiate(randomRoomToGenerateObject, referenceRoomPosition + (direction * _normalRoomSize * referenceRoomScript.roomSizeMultiplier), Quaternion.identity);

                            _occupiedPositions.Add(referenceRoomPosition + (direction * _normalRoomSize * referenceRoomScript.roomSizeMultiplier), generatedRoom.GetComponent<WorldRoom>());

                            WorldRoom generatedRoomScript = generatedRoom.GetComponent<WorldRoom>();
                            referenceRoomScript.AddDoor(direction, generatedRoomScript, false);
                            generatedRoomScript.AddDoor(direction * -1, referenceRoomScript, true);
                        }
                    }

                    if (generatedRoom != null)
                    {
                        break;
                    }
                }

                error++;
                if (error > 3000)
                {
                    Debug.LogError("Prevented an infinite loop on the map generator");
                    break;
                }
            }
        }
    }

    Vector2 GetRandomDirection()
    {
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                return Vector2.up;
            case 1:
                return Vector2.right;
            case 2:
                return Vector2.down;
            case 3:
                return Vector2.left;
            default:
                return Vector2.up;
        }
    }
}