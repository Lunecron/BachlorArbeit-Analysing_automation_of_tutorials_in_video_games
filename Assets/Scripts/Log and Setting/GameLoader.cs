using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameLoader : MonoBehaviour
{
    public string saveDirectory = "Settings";
    public string saveName = "settings";

    private void Start()
    {
        string path = saveDirectory + "/" + saveName + ".bin";

        if (File.Exists(path))
        {
            LoadFromFile();
            FindObjectOfType<SaveGame>().UpdateSaveGameData();
        }
    }

    public void LoadFromFile()
    {
        string path = saveDirectory + "/" + saveName + ".bin";

        if (!File.Exists(path))
        {
            Debug.Log(saveName + ":File not found!");
            return;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        FileStream saveFile = File.Open(saveDirectory + "/" + saveName + ".bin", FileMode.Open);

        SaveGameData loadData = (SaveGameData) formatter.Deserialize(saveFile);

        print("~~~~~~LOADED GAME DATA~~~~~~");
        print("MouseSenseX = " + loadData.mouseX);
        print("MouseSenseY = " + loadData.mouseY);
        print("~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        SetSaveGameData(loadData);


        saveFile.Close();
    }


    private void SetSaveGameData(SaveGameData loadedData)
    {
        PlayerCam playerCam = FindObjectOfType<PlayerCam>();
        playerCam.sensX = loadedData.mouseX;
        playerCam.sensY = loadedData.mouseY;

    }
}
