using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForTutorial : MonoBehaviour
{
    private bool inTutorialZone = false;
    private Tutorial activeTutorial;


    public void SetActiveTutorialZone(bool value)
    {
        inTutorialZone = value;
    }

    public bool inTutorial()
    {
        return inTutorialZone;
    }

    public Tutorial GetActiveTutorial()
    {
        if (!activeTutorial)
        {
            Debug.Log("Kein Actives Tutorial: Fehler bei " + gameObject);
            return null;
        }
        return activeTutorial;
    }

    public void SetActiveTutorial(Tutorial newTutorial)
    {
        activeTutorial = newTutorial;
    }
}
