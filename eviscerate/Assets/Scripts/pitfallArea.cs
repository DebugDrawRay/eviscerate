using UnityEngine;
using System.Collections;

public class pitfallArea : MonoBehaviour
{
    void OnTriggerEnter(Collider hit)
    {
        PlayerCharacter isPlayer = hit.GetComponent<PlayerCharacter>();

        if(isPlayer)
        {
            isPlayer.triggerFall();
        }
    }
}
