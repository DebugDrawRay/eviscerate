using UnityEngine;
using System.Collections;

public class slowMovementArea : MonoBehaviour
{
    public float slowFactor;

    void OnTriggerEnter(Collider hit)
    {
        PlayerCharacter isPlayer = hit.GetComponent<PlayerCharacter>();

        if(isPlayer)
        {
            isPlayer.changeSpeed(slowFactor);
        }
    }
    void OnTriggerExit(Collider hit)
    {
        PlayerCharacter isPlayer = hit.GetComponent<PlayerCharacter>();

        if (isPlayer)
        {
            isPlayer.changeSpeed(1);
        }
    }
}
