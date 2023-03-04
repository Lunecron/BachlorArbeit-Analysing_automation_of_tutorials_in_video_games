using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkPointNumber;
    private bool checkPointNumberFound = false;

    [SerializeField] RespawnZone respawnZone;
    [SerializeField] Transform respawnPoint;

    [SerializeField] public Vector3 lookDirectionWhileSpawning = new Vector3(0f,0f,0f);
    [SerializeField] float gizmoRadius = 0.5f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (!checkPointNumberFound)
        {
            GetCheckpointNumber();
        }
    }

    private void GetCheckpointNumber()
    {
        if(respawnZone.GetCheckpoints() == null)
        {
            Debug.Log("Respawn Zone hat keine Checkpoints : Fehler bei " + gameObject);
        }

        for(int i = 0; i< respawnZone.GetCheckpoints().Length; i++)
        {
            if(respawnZone.GetCheckpoints()[i].gameObject.name == gameObject.name)
            {
                checkPointNumber = i;
                checkPointNumberFound = true;
            }
            
        }

        if(checkPointNumberFound == false)
        {
            Debug.Log("Checkpointnummer konnte nicht gefunden werden. Fehler bei : " + gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkPointNumber > respawnZone.GetActiveCheckpointNumber())
        {
            respawnZone.SetActiveCheckpointNumber(checkPointNumber);
        }
    }

    public Transform GetRespawnPoint()
    {
        return respawnPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + lookDirectionWhileSpawning);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + lookDirectionWhileSpawning, gizmoRadius);
    }
}
