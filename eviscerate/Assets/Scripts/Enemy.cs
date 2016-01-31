using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public bool inKnockback
    {
        get;
        protected set;
    }
    protected Rigidbody rigid;

    [Header("Hit Reaction")]
    public float knockbackForce;
    public float knockbackLength;
    protected float currentLength;

    protected float damage;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    public void triggerKnockback(Vector3 source)
    {
        inKnockback = true;
        currentLength = knockbackLength;

        Vector3 dir = transform.position - source;
        dir.y = 0;
        rigid.AddForce(dir.normalized * knockbackForce);
    }
    public void knockback()
    {
        currentLength -= Time.deltaTime;
        if(currentLength <= 0)
        {
            inKnockback = false;

        }
    }

    void OnTriggerEnter(Collider hit)
    {
        PlayerCharacter isPlayer = hit.GetComponent<PlayerCharacter>();
        if(isPlayer)
        {
            isPlayer.changeStatus(damage, false, gameObject);
        }
    } 
}
