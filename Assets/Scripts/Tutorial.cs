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
    Use_Log_File log_file;

    [Header("Timer")]
    public float tutorialExecTime;
    private float tutorialExecTimer = 0f;

    private bool tutorialStarted = false;
    private bool inTutorial = false;
    private bool inRange = false;

    [Header("Check for")]
    public bool checkState = false;
    public PlayerMovement.MovementState stateToCheck;
    public bool checkBool = false;
    public string boolToCheck;
    public int resets;
    public int maxResetsTillTut = 1;

    [Header("Tutorial Text")]
    [SerializeField] public string tutorialTitel;
    [SerializeField] public string tutorialText;
    [SerializeField] public bool useImage = false;
    [SerializeField] public Sprite buttonImage;
    public KeyCode continueButton = KeyCode.Return;

    [Header("Check for same Tutorial")]
    [SerializeField] bool isButtonTutorial = false;

    [Header("HelpButton")]
    public KeyCode helpButtonKey = KeyCode.H;


    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        pm = player.GetComponent<PlayerMovement>();
        tutorialExecTimer = 0f;
        if (!log_file)
        {
            log_file = FindObjectOfType<Use_Log_File>();
        }

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

        if (FindObjectOfType<ButtonTutorialCheck>().buttonTutorial && isButtonTutorial &&!tutorialStarted)
        {
            SkipTutorial();
        }

        if (inRange && Input.GetKeyDown(helpButtonKey) && pm.grounded && !inTutorial)
        {
            StartTutorial();
        }



    }

    private void OnTriggerEnter(Collider other)
    {

        if (!tutorialStarted)
        {            
            player.GetComponent<CheckForTutorial>().SetActiveTutorial(gameObject.GetComponent<Tutorial>());
            
        }

        if (isButtonTutorial)
        {
            FindObjectOfType<ButtonTutorialCheck>().buttonTutorial = true;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player")
        {
            return;
        }

        inRange = true;

       
        if (!tutorialStarted)
        {
            if (tutorialExecTimer < tutorialExecTime)
            {
                tutorialExecTimer += Time.deltaTime;
            }

            if ((tutorialExecTimer >= tutorialExecTime) && pm.grounded)
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
            else if (resets >= maxResetsTillTut && pm.grounded)
            {
                StartTutorial();
            }
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }

    private void StartTutorial()
    {

        gameMenu.DisableHelpButton();

        log_file.LogString(gameObject.name, "Started");
        
        tutorialStarted = true;
        inTutorial = true;
        popUpSystem.PopUp(tutorialTitel,tutorialText,useImage);

        if (useImage)
        {
            SetButtonImage();
        }
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

        gameMenu.EnableHelpButton();
    }

    private void SetButtonImage()
    {
        GameObject button = FindObjectOfType<ButtonImage>().gameObject;
        button.GetComponent<Image>().sprite = buttonImage;
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
        else if(boolName =="walljuming")
        {
            return pm.walljumping;
        }
        else if (boolName == "wallclimbing")
        {
            return pm.wallclimbing;
        }
        else if (boolName == "sliding")
        {
            return pm.sliding;
        }
        else if (boolName == "swinging")
        {
            return pm.swinging;
        }
        else if (boolName == "grappling")
        {
            return pm.activeGrapple;
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
        else if (boolName == "moved")
        {
            return pm.moved;
        }

        return false;
    }

    private void SkipTutorial()
    {
        log_file.LogString(gameObject.name, "Skipped");
        tutorialStarted = true;

        gameMenu.EnableHelpButton();
    }

    public void increaseResets(int amount)
    {
        resets += amount;
        log_file.IncreaseDeath();
    }
}
