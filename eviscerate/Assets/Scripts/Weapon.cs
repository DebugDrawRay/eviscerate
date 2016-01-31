using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public float damage;

    public GameObject hitEffect;
    public GameObject slashEffect;

    void Start()
    {
        Instantiate(slashEffect, transform.position, transform.rotation);
    }
    void OnTriggerEnter(Collider hit)
    {
        Instantiate(hitEffect, transform.position, Quaternion.Euler(90, 0, 0));

        Status hasStatus = hit.GetComponent<Status>();
        Enemy isEnemy = hit.GetComponent<Enemy>();

        if(hasStatus)
        {
            hasStatus.changeStatus(damage);
        }

        if(isEnemy)
        {
            if (!isEnemy.inKnockback)
            {
                isEnemy.triggerKnockback(transform.position);
            }
        }
    }
}
