using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorMenuToggle : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject SelectorOptionsMenu, PrefabsSelectorMenu;
    void OnEnable()
    {
        EditsManager.OnObjectInstantiated += ToggleSelectorOptionsMenu;
    }

    void OnDisable()
    {
        EditsManager.OnObjectInstantiated -= ToggleSelectorOptionsMenu;
    }
    void ToggleSelectorOptionsMenu()
    {
        SelectorOptionsMenu.SetActive(true);
        gameObject.SetActive(false);
        SelectorOptionsMenu.GetComponent<PreviousMenuCallback>().SaveCallingMenu(gameObject);
    }

    public void TogglePrefabSelectorMenu()
    {
        PrefabsSelectorMenu.SetActive(true);
        gameObject.SetActive(false);
        PrefabsSelectorMenu.GetComponent<PreviousMenuCallback>().SaveCallingMenu(gameObject);
    }


}
