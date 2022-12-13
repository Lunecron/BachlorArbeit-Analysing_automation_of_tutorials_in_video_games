using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Checkpoints")]
    [SerializeField] private RespawnZone respawnZone;
    [SerializeField] private GameObject player;
    [SerializeField] private Checkpoint[] checkPoints;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        respawnZone = GetComponentInParent<RespawnZone>();
        
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag != "Player")
        {
            return;
        }
        checkPoints = respawnZone.GetCheckpoints();
        player.GetComponent<CheckForTutorial>().GetActiveTutorial().increaseResets(1);

        player.transform.position = checkPoints[respawnZone.GetActiveCheckpointNumber()].GetRespawnPoint().position;

    }


    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }
}
