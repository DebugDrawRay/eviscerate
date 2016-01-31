using UnityEngine;
using System.Collections;

public class RoomTransition : MonoBehaviour
{
    public Transform destination;
    public Transform newCheckpoint;

    void OnTriggerEnter(Collider hit)
    {
        PlayerCharacter isPlayer = hit.GetComponent<PlayerCharacter>();
        if (isPlayer)
        {
            PlayerCharacter.instance.transform.position = destination.position;
            GameController.instance.currentCheckpoint = newCheckpoint;
            Debug.Log("Moved");
        }
    }
}
