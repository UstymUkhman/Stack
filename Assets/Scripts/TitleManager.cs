using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    [Header("Title animation duration (in seconds):")]
    [SerializeField] private float animationDuration = 10.0f;

    private Color32 SOLID = new Color32(255, 255, 255, 255);
    private Color32 TRANSPARENT = new Color32(255, 255, 255, 0);

    private List<TextMeshProUGUI> titles = new List<TextMeshProUGUI>();

    private void Awake()
    {
        for (int c = 1; c < transform.childCount; c++)
        {
            TextMeshProUGUI title = transform.GetChild(c)
                .GetComponent<TextMeshProUGUI>();

            title.color = TRANSPARENT;
            titles.Add(title);
        }
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (TextMeshProUGUI title in titles)
        {
            StartCoroutine(Animate(title));
        }

        yield return new WaitForSeconds(1.5f);

        transform
            .GetChild(0)
            .GetComponent<RawImage>()
            .CrossFadeAlpha(0.0f, 1.0f, false);
    }

    private IEnumerator Animate(TextMeshProUGUI title)
    {
        float startTime = Time.time;
        float endTime = Time.time + animationDuration;

        RectTransform transform = title.transform.GetComponent<RectTransform>();
        Vector2 position = transform.anchoredPosition;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / animationDuration;

            transform.anchoredPosition = Vector2.LerpUnclamped(position, Vector2.zero, time);
            title.color = Color32.Lerp(TRANSPARENT, SOLID, time);
            yield return null;
        }

        transform.anchoredPosition = Vector2.zero;
        title.color = SOLID;
    }

    public void Dispose()
    {
        CanvasGroup group = transform.GetComponent<CanvasGroup>();

        if (group.alpha == 1.0f)
        {
            StartCoroutine(FadeOut(group));
        }
    }

    private IEnumerator FadeOut(CanvasGroup title)
    {
        float startTime = Time.time;
        float endTime = Time.time + 0.5f;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / 0.5f;
            title.alpha = Mathf.Lerp(1.0f, 0.0f, time);
            yield return null;
        }

        title.alpha = 0.0f;
        titles.Clear();
    }
}
