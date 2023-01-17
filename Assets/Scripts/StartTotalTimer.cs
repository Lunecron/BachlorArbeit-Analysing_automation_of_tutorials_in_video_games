using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTotalTimer : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        FindObjectOfType<Use_Log_File>().StartTimer();
        Debug.Log("Start Timer");
    }
}
