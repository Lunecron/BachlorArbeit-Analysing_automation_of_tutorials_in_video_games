using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBetweenSettings : MonoBehaviour
{
    [SerializeField] GameObject firstObjectGroup;
    [SerializeField] GameObject secondObjectGroup;

    public void ToggleGroups()
    {
        if (firstObjectGroup.activeSelf)
        {
            firstObjectGroup.SetActive(false);
            secondObjectGroup.SetActive(true);
        }
        else
        {
            firstObjectGroup.SetActive(true);
            secondObjectGroup.SetActive(false);
        }
        
    }
}
