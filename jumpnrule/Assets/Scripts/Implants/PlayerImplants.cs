using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImplants : MonoBehaviour
{
    public List<ImplantData> _implantsPool = new List<ImplantData>();
    protected List<int> _currentImplants = new List<int>();

    public virtual void Start()
    {
        GameEvents.Instance.onUpgradeChosen += OnImplantsObtained;
    }

    public virtual void OnDestroy()
    {
        GameEvents.Instance.onUpgradeChosen -= OnImplantsObtained;
    }

    public virtual void OnImplantsObtained(int id, int level)
    {
        if (!_currentImplants.Contains(id))
        {
            _currentImplants.Add(id);
        }
    }
}
