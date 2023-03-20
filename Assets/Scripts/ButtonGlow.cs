using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGlow : MonoBehaviour
{
    float glowTimer = 0;
    float intervalTimer = 0;
    [SerializeField] float glowTime = 3f;
    [SerializeField] float glowInterval = 3f;
    [SerializeField] float animationSpeed = 1f;
    [SerializeField] GameObject glowTexture;
    Color startColor;
    bool active = false;
    bool glowing = false;
    float alphaInceaseValue = 0;
    float alphaDeceaseValue = 0;

    // Start is called before the first frame update
    void Start()
    {
     
        startColor = gameObject.GetComponent<Image>().color;
        alphaInceaseValue = glowTime/255;
        alphaDeceaseValue = glowInterval/255;
    }

    private void FixedUpdate()
    {
        if (active)
        {

            if (glowTime > glowTimer && glowing == true)
            {
                glowTimer += Time.deltaTime;
                IncreaseAlphaValue(alphaInceaseValue);
            }
            else if(glowing == true)
            {
                glowing = false;
                intervalTimer = 0;
            }
            
            if(glowInterval > intervalTimer && glowing == false)
            {
                intervalTimer += Time.deltaTime;
                DecreaseAlphaValue(alphaInceaseValue);
            }
            else if(glowing == false)
            {
                glowing = true;
                glowTimer = 0;
            }
        }
    }

    private void IncreaseAlphaValue(float value)
    {
        if (glowTexture.GetComponent<Image>().color.a >= 255)
        {
            return;
        }
        glowTexture.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, glowTexture.GetComponent<Image>().color.a + value);
    }

    private void DecreaseAlphaValue(float value)
    {
        if (glowTexture.GetComponent<Image>().color.a <= 0)
        {
            return;
        }
        glowTexture.GetComponent<Image>().color = new Color(startColor.r, startColor.g, startColor.b, glowTexture.GetComponent<Image>().color.a - value);
    }

    private void OnEnable()
    {
        active = true;
    }

    private void OnDisable()
    {
        active = false;
    }
}
