using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpSystem : MonoBehaviour
{
    public GameObject popUpBox;
    public Animator popUpAnimator;

    public TMPro.TMP_Text popUpText;
    public TMPro.TMP_Text popUpTitel;

    public GameObject button_image;

    public void PopUp(string titel,string text, bool enableButtonImage)
    {

        if (enableButtonImage)
        {
            EnableButtonImage();
        }
        else
        {
            DisableButtonImage();
        }

        popUpBox.SetActive(true);
        popUpTitel.text = titel;
        popUpText.text = text;

        popUpAnimator.SetTrigger("pop");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void EnableButtonImage()
    {
        button_image.SetActive(true);
    }

    public void DisableButtonImage()
    {
        button_image.SetActive(false);
    }
}
