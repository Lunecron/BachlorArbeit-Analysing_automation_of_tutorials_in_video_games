using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallrunTimer : MonoBehaviour
{
    public GameObject player;
    private Wallrunning wallrunscript;
    private PlayerMovement pm;

    [SerializeField] private Image circleFill;
    [SerializeField] private TMPro.TMP_Text textMeshPro;

    private float timerFloat;
    private int timerInt;
    private float maxTimer;

    private void Start()
    {
        
        wallrunscript = player.GetComponent<Wallrunning>();
        pm = player.GetComponent<PlayerMovement>();
        maxTimer = wallrunscript.maxWallRunTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.wallrunning)
        {

            timerFloat = wallrunscript.wallRunTimer;
            timerInt = (int)timerFloat;
            textMeshPro.text = $"{timerInt % 60}";
            circleFill.fillAmount = Mathf.InverseLerp(0, maxTimer, timerFloat);
        }
        else
        {
            textMeshPro.text = $"{maxTimer}";
            circleFill.fillAmount = 1;
        }
        
    }
}
