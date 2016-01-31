using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
    public float health;
    public GameObject deathAnim;

    public void changeStatus(float healthChange)
    {
        Debug.Log("Ouch");
        health += healthChange;
        if(health <= 0)
        {
            deathEvent();
        }
    }

    void deathEvent()
    {
        if (deathAnim)
        {
            Instantiate(deathAnim, transform.position, Quaternion.Euler(90, 0, 0));
        }
        Destroy(gameObject);
    }
}
