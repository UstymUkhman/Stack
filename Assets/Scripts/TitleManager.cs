using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    private List<Text> titles = new List<Text>();
    private List<Vector2> initialPositions = new List<Vector2>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Text title = child.GetComponent<Text>();
            RectTransform rect = title.GetComponent<RectTransform>();

            initialPositions.Add(rect.anchoredPosition * 0.5f);
            title.color = ColorManager.TRANSPARENT;
            titles.Add(title);
        }
    }

    public IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Text title in titles)
        {
            StartCoroutine(Animate(title, true, 2.0f));
        }
    }

    public void FadeIn()
    {
        Reset();

        foreach (Text title in titles)
        {
            StartCoroutine(Animate(title, true));
        }
    }

    public void FadeOut()
    {
        foreach (Text title in titles)
        {
            StartCoroutine(Animate(title));
        }
    }

    private void Reset()
    {
        for (int t = 0; t < titles.Count; t++)
        {
            Text title = titles[t];
            RectTransform rect = title.GetComponent<RectTransform>();

            rect.anchoredPosition = initialPositions[t];
            title.color = ColorManager.TRANSPARENT;
        }
    }

    private IEnumerator Animate(Text title, bool visible = false, float duration = 0.5f)
    {
        float startTime = Time.time;
        float endTime = Time.time + duration;

        Color32 target = visible ? ColorManager.SEMI_WHITE : ColorManager.TRANSPARENT;
        Color32 current = visible ? ColorManager.TRANSPARENT : ColorManager.SEMI_WHITE;

        RectTransform rect = title.GetComponent<RectTransform>();
        Vector2 position = rect.anchoredPosition;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / duration;

            rect.anchoredPosition = Vector2.LerpUnclamped(position, Vector2.zero, time);
            title.color = Color32.Lerp(current, target, time);

            yield return null;
        }

        rect.anchoredPosition = Vector2.zero;
        title.color = target;
    }
}
