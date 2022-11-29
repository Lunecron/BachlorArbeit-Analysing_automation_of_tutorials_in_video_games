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

    private int timer;
    private int maxTimer;

    private void Start()
    {
        
        wallrunscript = player.GetComponent<Wallrunning>();
        pm = player.GetComponent<PlayerMovement>();
        maxTimer = (int)wallrunscript.maxWallRunTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.wallrunning)
        {
            timer = (int)wallrunscript.wallRunTimer;
            textMeshPro.text = $"{timer % 60}";
            circleFill.fillAmount = Mathf.InverseLerp(0, maxTimer, timer);
        }
        else
        {
            textMeshPro.text = $"{maxTimer}";
            circleFill.fillAmount = 1;
        }
        
    }
}
