using UnityEngine;
using System.Collections;

public class ChaseCamera : MonoBehaviour
{
    [Range(0,1)]
    public float lazyness;
    private float defaultFloat;

    void Start()
    {
        defaultFloat = transform.position.y;
    }

    void FixedUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, PlayerCharacter.instance.transform.position, lazyness);
        newPosition.y = defaultFloat;
        transform.position = newPosition;
    }
}
