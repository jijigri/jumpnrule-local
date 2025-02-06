using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasPlayerOwner
{
    public void SetPlayerOwner(GameObject playerOwner);
    public GameObject GetPlayerOwner();
}
