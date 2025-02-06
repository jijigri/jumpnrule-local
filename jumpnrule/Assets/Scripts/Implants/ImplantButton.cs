using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImplantButton : MonoBehaviour
{
    bool isActive = true;

    [HideInInspector] public int upgradeIndex = 0;

    public void OnButtonPress()
    {
        if (isActive)
        {
            ImplantsManager.Instance.AddUpgrade(upgradeIndex);
        }
    }

    public void DisableButton()
    {
        isActive = false;
        GetComponent<Button>().interactable = false;

        Destroy(transform.GetComponentInParent<ImplantDisplay>().gameObject, .5f);
    }
}
