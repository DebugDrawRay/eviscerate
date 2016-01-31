using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class hurtMeter : MonoBehaviour
{
    private float targetOpacity;
    private Image img;
    private Color imgColor;
    private PlayerCharacter player;
	void Start ()
    {
        player = PlayerCharacter.instance;
        img = GetComponent<Image>();
        targetOpacity = img.color.a;
        imgColor = img.color;
        imgColor.a = 0;
        img.color = imgColor;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float currentOpacity = targetOpacity * (1 - (player.health / player.maxHealth));
        imgColor.a = currentOpacity;
        img.color = imgColor;
	}
}
