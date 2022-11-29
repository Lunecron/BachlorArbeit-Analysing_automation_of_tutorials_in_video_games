using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpSystemHook : MonoBehaviour
{
    public PopUpSystem popUpSystem;

    public void Resume()
    {
        popUpSystem.Resume();
    }

    public void Pause()
    {
        popUpSystem.Pause();
    }
}
