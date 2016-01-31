using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifetime;

    void Update()
    {
        GetComponent<Rigidbody>().velocity = transform.up * speed;

        lifetime -= Time.deltaTime;

        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        PlayerCharacter isPlayer = hit.GetComponent<PlayerCharacter>();
        if (isPlayer)
        {
            isPlayer.changeStatus(damage, false, gameObject);
            Destroy(gameObject);
        }
    }
}
