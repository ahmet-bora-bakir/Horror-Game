using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public ControlPlayer playerScript; 
    public Image vignetteImage;        

    [Header("Colors")]
    public Color normalColor = Color.black;
    public Color tiredColor = Color.red;

    [Header("Settings")]
    public float baseOpacity = 0f; 
    public float maxOpacity = 1f;  

    void Update()
    {
        if (playerScript == null || vignetteImage == null) return;

        
        float staminaMissing = 1f - (playerScript.stamina / 10f);

        
        Color targetColor = playerScript.is_tired ? tiredColor : normalColor;

       
        vignetteImage.color = Color.Lerp(vignetteImage.color, targetColor, Time.deltaTime * 3f);

        
        float targetAlpha = Mathf.Lerp(baseOpacity, maxOpacity, staminaMissing);
        
       
        if (playerScript.is_tired)
        {
            targetAlpha += Mathf.Sin(Time.time * 8f) * 0.15f;
        }

        Color finalCol = vignetteImage.color;
        finalCol.a = Mathf.Clamp01(targetAlpha);
        vignetteImage.color = finalCol;
    }
}