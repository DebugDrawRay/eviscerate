using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public float damage;

    void OnTriggerEnter(Collider hit)
    {
        Status hasStatus = hit.GetComponent<Status>();

        if(hasStatus)
        {
            hasStatus.changeStatus(damage);
        }
    }
}
