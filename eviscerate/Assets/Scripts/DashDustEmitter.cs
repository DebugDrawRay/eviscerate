using UnityEngine;
using System.Collections;

public class DashDustEmitter : MonoBehaviour
{
    public float emitInterval;
    private float currentInterval;

    public GameObject dust;

    void Start()
    {
        currentInterval = emitInterval;
    }
    void Update()
    {
        currentInterval -= Time.deltaTime;
        if(currentInterval <= 0)
        {
            Instantiate(dust, transform.position, Quaternion.Euler(90,0,0));
            currentInterval = emitInterval;
        }
    }
}
