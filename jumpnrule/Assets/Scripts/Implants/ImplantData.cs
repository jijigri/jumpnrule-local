using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Implant", menuName = "Custom/Implant")]
public class ImplantData : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite sprite;

    public int id;
    public int level = 0;

    [TextArea(15, 20)]
    [SerializeField] private string notes = "Insert private notes";
}
