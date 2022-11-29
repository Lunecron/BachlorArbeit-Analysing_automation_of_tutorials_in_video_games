using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    private PlayerMovement pm;
    public PopUpSystem popUpSystem;
    public Animator popUpBox;
    public GameMenu gameMenu;
    [Header("Timer")]
    public float tutorialExecTime;
    private float tutorialExecTimer = 0f;

    private bool tutorialStarted = false;
    private bool inTutorial = false;

    [Header("Check for")]
    public bool checkState = false;
    public PlayerMovement.MovementState stateToCheck;
    public bool checkBool = false;
    public string boolToCheck;
    public int resets;
    public int maxResetsTillTut = 1;

    [Header("Tutorial Text")]
    [SerializeField] public string tutroialText;
    public KeyCode continueButton = KeyCode.Escape;


    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        pm = player.GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        if (inTutorial)
        {
            if (Input.GetKeyDown(continueButton))
            {
                EndTutorial();
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        tutorialExecTimer = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        tutorialExecTimer += Time.deltaTime;
        if ((tutorialExecTimer >= tutorialExecTime) && !tutorialStarted)
        {
            StartTutorial();
        }
        else if (PlayerStateReached(stateToCheck) && checkState)
        {
            SkipTutorial();
        }
        else if (BoolToCheck(boolToCheck) && checkBool)
        {
            SkipTutorial();
        }
        else if(resets >= maxResetsTillTut && !tutorialStarted)
        {
            StartTutorial();
        }
    }

    private void StartTutorial()
    {
        tutorialStarted = true;
        inTutorial = true;
        popUpSystem.PopUp(tutroialText);
        gameMenu.EnableGameMenu(false);
        if (!player)
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }
        player.SetActive(false);
    }

    private void EndTutorial()
    {
        popUpSystem.Resume();
        inTutorial = false;
        popUpBox.SetTrigger("closed");
        if (!player)
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }
        player.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(EnableGameMenuAfterDelay(true,1f));
        Invoke(nameof(StopMyCoroutine),1f);
        
    }

    private void StopMyCoroutine()
    {
        StopCoroutine(EnableGameMenuAfterDelay(true, 1f));
    }

    IEnumerator EnableGameMenuAfterDelay(bool value, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gameMenu.EnableGameMenu(value);
        
    }

    private bool PlayerStateReached(PlayerMovement.MovementState state)
    {
        if(pm.state == state)
        {
            return true;
        }
        return false;
    }

    private bool BoolToCheck(string boolName)
    {
        if(boolName == "wallrunning")
        {
            return pm.wallrunning;
        }
        else if (boolName == "sliding")
        {
            return pm.sliding;
        }
        else if (boolName == "swinging")
        {
            return pm.swinging;
        }
        else if (boolName == "walking")
        {
            return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
        }
        else if (boolName == "restricted")
        {
            //dunno yet
        }
        else if (boolName == "crouching")
        {
            return pm.crouching;
        }
        else if (boolName == "air")
        {
            return Input.GetKeyDown(pm.jumpKey);
        }

        return false;
    }

    private void SkipTutorial()
    {
        tutorialStarted = true;
    }

    public void increaseResets(int amount)
    {
        resets += amount;
    }
}
