using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Checkpoints")]
    [SerializeField] private RespawnZone respawnZone;
    [SerializeField] private GameObject player;
    [SerializeField] private Checkpoint[] checkPoints;

    private PlayerCam cam;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        respawnZone = GetComponentInParent<RespawnZone>();
        cam = FindObjectOfType<PlayerCam>();
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag != "Player")
        {
            return;
        }
        checkPoints = respawnZone.GetCheckpoints();
        if (player.GetComponent<CheckForTutorial>().GetActiveTutorial())
        {
            player.GetComponent<CheckForTutorial>().GetActiveTutorial().increaseResets(1);
        }

        player.transform.position = checkPoints[respawnZone.GetActiveCheckpointNumber()].GetRespawnPoint().position;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cam.LookAt(checkPoints[respawnZone.GetActiveCheckpointNumber()].lookDirectionWhileSpawning);
    }

}
