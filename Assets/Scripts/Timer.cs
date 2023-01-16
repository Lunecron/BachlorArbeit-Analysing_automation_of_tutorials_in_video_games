using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] string timername = "";
    [SerializeField] float timer = 0;
    [SerializeField] bool activeTimer = false;
    [SerializeField] Use_Log_File log_file;
    private void Start()
    {
        if (!log_file)
        {
            log_file = FindObjectOfType<Use_Log_File>();
        }
        
    }

    private void Update()
    {
        if (activeTimer)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartTimer();
    }

    public void StartTimer()
    {
        activeTimer = true;
    }

    public void StopTimer()
    {
        activeTimer = false;
    }

    public void SaveTimer()
    {
        log_file.LogTime(timername,timer);
    }
}
