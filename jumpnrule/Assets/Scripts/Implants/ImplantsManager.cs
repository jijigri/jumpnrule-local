using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantsManager : MonoBehaviour
{
    public static ImplantsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private List<ImplantData> upgradesPool = new List<ImplantData>();
    [SerializeField] private GameObject cardPrefab = null;
    [SerializeField] private Transform upgradesPanel = null;
    [SerializeField] private int numberOfUpgradeChoices = 3;
    Dictionary<int, int> upgrades = new Dictionary<int, int>();

    public void FillPanel()
    {
        for (int i = 0; i < numberOfUpgradeChoices; i++)
        {
            InstantiateCard();
        }
    }

    void InstantiateCard()
    {
        ImplantData randUpgrade = upgradesPool[Random.Range(0, upgradesPool.Count)];

        GameObject card = Instantiate(cardPrefab, upgradesPanel.transform.position, Quaternion.identity);
        card.transform.SetParent(upgradesPanel.transform, false);

        ImplantDisplay display = card.GetComponent<ImplantDisplay>();

        if (display != null)
        {
            display.SetUpgradeData(randUpgrade);
        }
    }

    public void AddUpgrade(int upgradeId)
    {
        int level = 0;

        if (upgrades.ContainsKey(upgradeId) == false)
        {
            upgrades.Add(upgradeId, 0);
            level = 0;
        }
        else
        {
            upgrades[upgradeId] = upgrades[upgradeId] + 1;
            level = upgrades[upgradeId];
        }

        GameEvents.Instance.UpdateChosen(upgradeId, level);

        HudManager.Instance.SetImplantPanel(false);

        Debug.Log("Upgrade Chosen at level " + level);
    }
}
