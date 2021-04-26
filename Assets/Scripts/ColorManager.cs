using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour
{
    private System.Random random = new System.Random();
    private Color[] platformColors = new Color[2];

    private static ColorManager instance;
    private int lastPlatformIndex;
    private int colorsAmount = 0;


public static ColorManager Instance {
        get {
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            SetPlatformColors(0);
            instance = this;
        }
    }

    public Color GetRandomColor() =>
        Random.ColorHSV(0.0f, 1.0f, 0.1f, 1.0f, 0.25f, 1.0f);

    private void SetPlatformColors(int index)
    {
        lastPlatformIndex = index;
        colorsAmount += random.Next(5, 25);

        platformColors[0] = GetRandomColor();
        platformColors[1] = GetRandomColor();
    }

    public Color GetPlatformColor(int platformIndex)
    {
        if (platformIndex == colorsAmount)
        {
            SetPlatformColors(platformIndex);
        }

        return Color.Lerp(
            platformColors[0],
            platformColors[1],
            1.0f / colorsAmount * (
                platformIndex - lastPlatformIndex
            )
        );
    }

    void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }
}
