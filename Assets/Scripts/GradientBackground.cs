using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[System.Serializable]
public class GradientBackground : MonoBehaviour
{
    public Color topColor = new Color(0.17f, 0.24f, 0.31f, 1f);
    public Color bottomColor = new Color(0.20f, 0.60f, 0.86f, 1f);
    
    [Range(0f, 360f)]
    public float angle = 135f;
    
    private Image imageComponent;

    void Start()
    {
        CreateGradient();
    }

    void OnValidate()
    {
        if (Application.isPlaying) return;
        CreateGradient();
    }

    void Awake()
    {
        CreateGradient();
    }

    void CreateGradient()
    {
        if (imageComponent == null)
            imageComponent = GetComponent<Image>();
        
        if (imageComponent == null) return;
        
        Texture2D gradientTexture = CreateGradientTexture();
        imageComponent.sprite = Sprite.Create(gradientTexture, 
            new Rect(0, 0, gradientTexture.width, gradientTexture.height), 
            Vector2.zero);
    }

    Texture2D CreateGradientTexture()
    {
        int width = 256;
        int height = 256;
        Texture2D texture = new Texture2D(width, height);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float normalizedX = (float)x / width;
                float normalizedY = (float)y / height;
                
                float gradientValue = (normalizedX + normalizedY) * 0.5f;
                gradientValue = Mathf.Clamp01(gradientValue);
                
                Color pixelColor = Color.Lerp(topColor, bottomColor, gradientValue);
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        return texture;
    }

    public void UpdateColors(Color newTopColor, Color newBottomColor)
    {
        topColor = newTopColor;
        bottomColor = newBottomColor;
        CreateGradient();
    }
}