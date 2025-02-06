using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOwnedObject : MonoBehaviour
{
    public GameObject Owner { get; set; }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
}
