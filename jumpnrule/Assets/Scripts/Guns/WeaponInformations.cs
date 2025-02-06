using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInformations : MonoBehaviour
{
    public new string name = "Weapon";
    [TextArea(10, 10)]
    public string description = "This is a weapon";
    public Sprite sprite = null;
}
