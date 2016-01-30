using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Camera Control")]
    public Camera currentCamera;


    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }

    public void changeCurrentCamera(Camera changeTo)
    {
        currentCamera.enabled = false;
        currentCamera = changeTo;
        currentCamera.enabled = true;
    }
}
