using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorMenuToggle : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject SelectorMenu;
    void OnEnable()
    {
        EditsManager.OnObjectInstantiated += ToggleSelectorMenu;
    }

    void OnDisable()
    {
        EditsManager.OnObjectInstantiated -= ToggleSelectorMenu;
    }
    void ToggleSelectorMenu()
    {
        SelectorMenu.SetActive(true);
        gameObject.SetActive(false);
    }


}
