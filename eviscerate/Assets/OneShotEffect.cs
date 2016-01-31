using UnityEngine;
using System.Collections;

public class OneShotEffect : MonoBehaviour
{
    public float lifeTime;

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
