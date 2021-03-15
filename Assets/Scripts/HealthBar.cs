using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image[] hearts;
    [SerializeField] Image heartImage;      
    [SerializeField] float heartWidth = 2f;

    [SerializeField] float anchorX;
    [SerializeField] float anchorY;

    // Start is called before the first frame update
    void Start()
    {        
        hearts = new Image[GameState.PlayerMaxHealth];
        for (int i = 0; i < GameState.PlayerMaxHealth; i++)
        {
            Image heart = Instantiate(heartImage, this.gameObject.transform, false);            
            heart.transform.position = new Vector3(anchorX + (heartWidth * i),
                anchorY, transform.position.z);
            hearts[i] = heart;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < GameState.PlayerMaxHealth; i++)
        {
            Image heart = hearts[i];
            if (i < GameState.Instance.PlayerHealth)
            {
                heart.enabled = true;
            }
            else
            {
                heart.enabled = false;
            }            
        }
    }
}
