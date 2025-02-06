using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasGunOwner
{
    public void SetGunOwner(Gun gunOwner);
    public Gun GetGunOwner();
}
