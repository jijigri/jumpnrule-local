using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WorldRoom : MonoBehaviour
{
    [SerializeField] private string _upDoorName = "UpDoor";
    [SerializeField] private string _rightDoorName = "RightDoor";
    [SerializeField] private string _downDoorName = "DownDoor";
    [SerializeField] private string _leftDoorName = "LeftDoor";
    public Vector2 roomSizeMultiplier = Vector2.one;

    public EnemySpawner EnemySpawner { get; private set; }

    List<Transform> _doors = new List<Transform>();
    Dictionary<DoorScript, DoorScript> _neighboringDoors = new Dictionary<DoorScript, DoorScript>();

    public DoorConnections doorConnections;

    CinemachineConfiner2D _cinemachineConfiner;

    private void Awake()
    {
        EnemySpawner = GetComponentInChildren<EnemySpawner>();
    }

    private void Start()
    {
        _cinemachineConfiner = GameObject.FindWithTag("CinemachineCamera").GetComponent<CinemachineConfiner2D>();
    }

    public void AddDoor(Vector2 direction, WorldRoom neighboringRoom, bool isNewRoom)
    {
        DoorScript door = GetDoorFromDirection(direction);
        door.CreateDoor(this);

        DoorScript neighboringDoor = neighboringRoom.GetDoorFromDirection(direction * -1);

        _neighboringDoors.Add(door, neighboringDoor);
    }

    public void UnlockRoom(DoorScript doorToOpen = null)
    {
        if (doorToOpen != null)
        {
            doorToOpen.UnlockDoor(doorToOpen);
        }

        CreateConfiner();
        RoundManager.Instance.AddRoomToPool(this);
    }

    public void UnlockNeighboringRoom(DoorScript door)
    {
        Debug.Log("Unlocking neighbor");
        if (_neighboringDoors.ContainsKey(door))
        {
            _neighboringDoors[door].GetParentRoom().UnlockRoom(_neighboringDoors[door]);
        }
    }

    void CreateConfiner()
    {
        GameObject confiner = new GameObject(gameObject.name + "Confiner");
        BoxCollider2D collider = confiner.AddComponent<BoxCollider2D>();

        collider.size = new Vector2(42, 24) * roomSizeMultiplier;
        collider.usedByComposite = true;
        collider.isTrigger = true;

        Instantiate(confiner, transform.position, Quaternion.identity, GameObject.FindWithTag("CameraConfiners").transform);

        if (_cinemachineConfiner != null)
        {
            _cinemachineConfiner.InvalidateCache();
        }
    }

    public DoorScript GetDoorFromDirection(Vector2 direction)
    {
        Transform doorsParent = transform.Find("Doors");

        if(direction == Vector2.up)
        { 
            if (doorsParent.transform.Find(_upDoorName))
            {
                Transform door = doorsParent.transform.Find(_upDoorName);
                doorConnections.up = true;
                return door.GetComponent<DoorScript>();
            }
        }
        else if(direction == Vector2.right)
        {
            if (doorsParent.transform.Find(_rightDoorName))
            {
                Transform door = doorsParent.transform.Find(_rightDoorName);
                doorConnections.right = true;
                return door.GetComponent<DoorScript>();
            }
        }
        else if(direction == Vector2.down)
        {
            if (doorsParent.transform.Find(_downDoorName))
            {
                Transform door = doorsParent.transform.Find(_downDoorName);
                doorConnections.down = true;
                return door.GetComponent<DoorScript>();
            }
        }
        else
        {
            if (doorsParent.transform.Find(_leftDoorName))
            {
                Transform door = doorsParent.transform.Find(_leftDoorName);
                doorConnections.left = true;
                return door.GetComponent<DoorScript>();
            }
        }

        return null;
    }

    public void StartRound()
    {
        Debug.Log("Received message from RoundManager ");
    }
}

public struct DoorConnections
{
    public bool up;
    public bool right;
    public bool down;
    public bool left;
}
