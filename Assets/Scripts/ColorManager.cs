using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private System.Random random = new System.Random();
    private Color[] platformColors = new Color[3];

    private static ColorManager instance;
    private int lastPlatformIndex;
    private int colorsAmount = 0;
    private int colorBlocks;

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

    public Color[] GetPlatformColors() =>
        new Color[] { platformColors[0], platformColors[2] };

    private void SetPlatformColors(int index)
    {
        colorBlocks = random.Next(5, 25);
        colorsAmount += colorBlocks;
        lastPlatformIndex = index;

        platformColors[0] = index == 0
            ? GetRandomColor()
            : platformColors[1];

        platformColors[1] = index == 0
            ? GetRandomColor()
            : platformColors[2];

        platformColors[2] = GetRandomColor();
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
            1.0f / (colorBlocks - 1) * (
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
