using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Use_Log_File : MonoBehaviour
{
    string path = "";
    int deathcounter = 0;
    float timer = 0;
    bool timerActive = false;
    void CreateText()
    {
        //Path of File
        path = Application.dataPath + "/Log.txt";
        //Create File
        if (!File.Exists(path))
        {
            File.WriteAllText(path,"Logging Information \n");
        }

        //Content
        string content = "Login date: " + System.DateTime.Now + "\n";

        //Add some text to it

        File.AppendAllText(path,content);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateText();
        
        
    }

    private void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        timerActive = true;
    }

    public void LogString(string name,string str)
    {
        File.AppendAllText(path,name+ ":" + str + "\n");
    }

    public void LogInt(string name, int value)
    {
        File.AppendAllText (path,name+ ":" + value + "\n");
    }

    public void LogTime(string name, float time)
    {
        File.AppendAllText(path,name + ":" + time + "\n");
        LogDeath(name + "_Deaths",deathcounter);
    }
    public void LogDeath(string name, int value)
    {
        File.AppendAllText(path, name + ":" + value + "\n");
        deathcounter = 0;
    }

    public void IncreaseDeath()
    {
        deathcounter++;
    }

    private void OnApplicationQuit()
    {
        File.AppendAllText(path, "Total_Time:" + timer + "\n");
    }


}
