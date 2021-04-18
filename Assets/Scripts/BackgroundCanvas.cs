using UnityEngine;
using UnityEngine.UI;

public class BackgroundCanvas : MonoBehaviour
{
    private RawImage background;
    private Texture2D texture;

    void Awake()
    {
        texture = new Texture2D(1, 2);
        background = transform.GetChild(0).GetComponent<RawImage>();

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        SetGradient(Color.white, Color.black);
    }

    public void SetGradient(Color top, Color bottom)
    {
        Color[] gradient = new Color[] { bottom, top };

        texture.SetPixels(gradient);
        texture.Apply();

        background.texture = texture;
    }
}
