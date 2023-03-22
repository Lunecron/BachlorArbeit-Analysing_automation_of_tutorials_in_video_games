using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGame : MonoBehaviour
{
    [SerializeField] string saveName = "settings";
    [SerializeField] string directorName = "Settings";
    public SaveGameData saveGameData;

    private void Start()
    {
        if (!Directory.Exists(directorName))
        {
            SaveToFile();
        }
    }

    public void SaveToFile()
    {
        if (!Directory.Exists(directorName)){
            Directory.CreateDirectory(directorName);
        }

        BinaryFormatter formatter = new BinaryFormatter();

        string path = directorName + "/" + saveName + ".bin";

        FileStream saveFile = File.Create(path);

        UpdateSaveGameData();

        formatter.Serialize(saveFile, saveGameData);

        saveFile.Close();

        print("Game Saved to " + Directory.GetCurrentDirectory().ToString() + "/" + path);
    }

    public void UpdateSaveGameData()
    {
        PlayerCam playerCam = FindObjectOfType<PlayerCam>();
        saveGameData.mouseX = playerCam.sensX;
        saveGameData.mouseY = playerCam.sensY;

    }
}
