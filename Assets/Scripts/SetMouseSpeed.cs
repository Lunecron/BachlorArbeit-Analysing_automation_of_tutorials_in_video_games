using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMouseSpeed : MonoBehaviour
{
    [SerializeField] PlayerCam cam;
    [SerializeField] Slider sliderX;
    [SerializeField] Slider sliderY;
    [SerializeField] TMPro.TMP_InputField inputX;
    [SerializeField] TMPro.TMP_InputField inputY;

    private void Start()
    {
        cam = FindObjectOfType<PlayerCam>();
        UpdateSlider();
        UpdateInputFields();
        GameLoader gameLoader = FindObjectOfType<GameLoader>();
        if (gameLoader != null)
        {
            gameLoader.LoadFromFile();
        }
    }

    public void SetYSense(string newSense)
    {
       int newSensInt = 0;
       int.TryParse(newSense, out newSensInt);
        if (newSensInt != 0)
        {
            cam.sensY = newSensInt;
        }

        UpdateInputFields();
        UpdateSlider();
    }

    public void SetXSense(string newSense)
    {
        int newSensInt = 0;
        int.TryParse(newSense, out newSensInt);
        if (newSensInt != 0)
        {
            cam.sensX = newSensInt;
        }

        UpdateInputFields();
        UpdateSlider();
    }

    public void SliderSetYSense()
    {
        if (sliderY)
        {
            cam.sensY = sliderY.value;
        }
        UpdateInputFields();
        UpdateSlider();

    }

    public void SliderSetXSense()
    {
        if (sliderX)
        {
            cam.sensX = sliderX.value;
        }

        UpdateInputFields();
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        sliderX.value = cam.sensX;
        sliderY.value = cam.sensY;
    }

    private void UpdateInputFields()
    {
        inputX.text = cam.sensX.ToString();
        inputY.text = cam.sensY.ToString();
    }
}
