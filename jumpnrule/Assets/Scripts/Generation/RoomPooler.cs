using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPooler : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomsInPool = new List<GameObject>();

    public GameObject GetRandomRoom()
    {
        return _roomsInPool[Random.Range(0, _roomsInPool.Count)];
    }

    /*
    public GameObject GetRandomRoom(DoorDirection requiredDoor)
    {
        foreach(GameObject currentRoom in _roomsInPool)
        {
            WorldRoom worldRoom = currentRoom.GetComponent<WorldRoom>();
            bool hasRequiredDoor = CheckForRequiredDoor(worldRoom, requiredDoor);

            if (hasRequiredDoor)
            {
                return currentRoom;
            }
        }

        Debug.LogError("Error: Couldn't find a room with the correct door position");
        return null;
    }

    bool CheckForRequiredDoor(WorldRoom room, DoorDirection requiredDoor)
    {
        switch (requiredDoor)
        {
            case DoorDirection.UP:

                if (room.GetDoorConnections().up)
                {
                    return true;
                }

                break;

            case DoorDirection.RIGHT:

                if (room.GetDoorConnections().right)
                {
                    return true;
                }

                break;

            case DoorDirection.DOWN:

                if (room.GetDoorConnections().down)
                {
                    return true;
                }

                break;

            case DoorDirection.LEFT:

                if (room.GetDoorConnections().left)
                {
                    return true;
                }

                break;
        }

        return false;
    }
    */
}
