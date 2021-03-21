using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image[] hearts;
    [SerializeField] Image heartImage;      
    private float heartWidth = 0f;

    [SerializeField] float xOffsetPct = 0.05f;
    [SerializeField] float yOffsetPct = 0.05f;

    CanvasScaler canvasScaler;

    // Start is called before the first frame update
    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        float xOffset = canvasScaler.referenceResolution.x * xOffsetPct;
        float yOffset = canvasScaler.referenceResolution.y * yOffsetPct;

        heartWidth = heartImage.preferredWidth * 1.0f;
        // no need to compare to screen as offset from left
        float anchorX = xOffset;
        // need to substract from height as offset from top
        float anchorY = Screen.height - yOffset;

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
