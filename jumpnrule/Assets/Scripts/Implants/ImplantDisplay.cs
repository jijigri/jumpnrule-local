using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImplantDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI descriptionText = null;
    [SerializeField] private Image sprite = null;
    [SerializeField] private ImplantButton button = null;

    public void SetUpgradeData(ImplantData upgrade)
    {
        nameText.text = upgrade.name;
        descriptionText.text = upgrade.description;
        sprite.sprite = upgrade.sprite;
        button.upgradeIndex = upgrade.id;
    }
}
