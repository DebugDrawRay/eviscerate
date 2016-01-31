using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
    public float health;

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
        Destroy(gameObject);
    }
}
