using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundCanvas : MonoBehaviour
{
    [Header("Background gradient color cross fade duration (in seconds):")]
    [SerializeField] private float crossFadeDuration = 5.0f;

    [Header("Background gradient color change frequency (in seconds):")]
    [SerializeField] private float backgroundDelay = 60.0f;

    private RawImage[] background = new RawImage[2];
    private int currentIndex = 0;

    private void Awake()
    {
        background[0] = transform.GetChild(0).GetComponent<RawImage>();
        background[1] = transform.GetChild(1).GetComponent<RawImage>();
    }

    private void Start()
    {
        SetBackgroundTexture(0);
        SetBackgroundTexture(1);

        background[1].CrossFadeAlpha(0.0f, 0.0f, true);
        StartCoroutine(ChangeBackgroundGradient());
    }

    private void SetBackgroundTexture(int index)
    {
        Texture2D texture = new Texture2D(1, 2);
        Color[] colors = ColorManager.GetPlatformColors();

        texture.SetPixels(new Color[] { colors[0], colors[1] });

        texture.Apply();
        background[index].texture = texture;

        background[index].texture.wrapMode = TextureWrapMode.Clamp;
        background[index].texture.filterMode = FilterMode.Bilinear;
    }

    private IEnumerator ChangeBackgroundGradient()
    {
        yield return new WaitForSeconds(backgroundDelay);

        int nextIndex = (currentIndex - 1) * -1;
        SetBackgroundTexture(nextIndex);

        background[currentIndex].CrossFadeAlpha(0.0f, crossFadeDuration, false);
        background[nextIndex].CrossFadeAlpha(1.0f, crossFadeDuration, false);

        currentIndex = nextIndex;
        StartCoroutine(ChangeBackgroundGradient());
    }
}
