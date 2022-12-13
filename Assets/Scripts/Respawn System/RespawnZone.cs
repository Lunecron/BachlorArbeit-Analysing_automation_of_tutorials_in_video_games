using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    [Header("Checkpoints")]
    [SerializeField] private Checkpoint[] checkPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private int activeCheckpointNumber = 0;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        checkPoints = gameObject.GetComponentsInChildren<Checkpoint>();
    }




    public void SetActiveCheckpointNumber(int checkPointNumber)
    {
        activeCheckpointNumber = checkPointNumber;
    }

    public int GetActiveCheckpointNumber()
    {
        return activeCheckpointNumber;
    }

    public Checkpoint[] GetCheckpoints()
    {
        return checkPoints;
    }

    public void resetCheckpoints()
    {
        activeCheckpointNumber = 0;
    }


}
