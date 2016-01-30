using UnityEngine;
using System.Collections;

public class RoomTransition : MonoBehaviour
{
    public Camera newCamera;

    void Start()
    {
        if(GameController.instance.currentCamera != newCamera)
        {
            newCamera.enabled = false;
        }
    }
    void OnTriggerEnter(Collider hit)
    {
        Debug.Log("Hit");
        if(hit.gameObject.tag == "Player")
        {
            GameController.instance.changeCurrentCamera(newCamera);
        }
    }
}
