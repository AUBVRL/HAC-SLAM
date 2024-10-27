using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousMenuCallback : MonoBehaviour
{
    GameObject callingMenu;

    public void SaveCallingMenu(GameObject previousMenu)
    {
        callingMenu = previousMenu;
    }

    public void GoBack()
    {
        callingMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
