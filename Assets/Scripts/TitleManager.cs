using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    private List<Vector2> initialPositions = new List<Vector2>();
    private List<TextMeshProUGUI> titles = new List<TextMeshProUGUI>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            TextMeshProUGUI title = child.GetComponent<TextMeshProUGUI>();
            RectTransform rect = title.transform.GetComponent<RectTransform>();

            initialPositions.Add(rect.anchoredPosition * 0.5f);
            title.color = ColorManager.TRANSPARENT;
            titles.Add(title);
        }
    }

    public IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (TextMeshProUGUI title in titles)
        {
            StartCoroutine(Animate(title, true, 2.0f));
        }
    }

    public void FadeIn()
    {
        Reset();

        foreach (TextMeshProUGUI title in titles)
        {
            StartCoroutine(Animate(title, true));
        }
    }

    public void FadeOut()
    {
        foreach (TextMeshProUGUI title in titles)
        {
            StartCoroutine(Animate(title));
        }
    }

    private void Reset()
    {
        for (int t = 0; t < titles.Count; t++)
        {
            Transform title = titles[t].transform;
            RectTransform rect = title.GetComponent<RectTransform>();

            rect.anchoredPosition = initialPositions[t];
            titles[t].color = ColorManager.TRANSPARENT;
        }
    }

    private IEnumerator Animate(TextMeshProUGUI title, bool visible = false, float duration = 0.5f)
    {
        float startTime = Time.time;
        float endTime = Time.time + duration;

        RectTransform transform = title.transform.GetComponent<RectTransform>();
        Vector2 position = transform.anchoredPosition;

        Color32 target = visible ? ColorManager.SOLID_WHITE : ColorManager.TRANSPARENT;
        Color32 current = visible ? ColorManager.TRANSPARENT : ColorManager.SOLID_WHITE;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / duration;

            transform.anchoredPosition = Vector2.LerpUnclamped(position, Vector2.zero, time);
            title.color = Color32.Lerp(current, target, time);

            yield return null;
        }

        transform.anchoredPosition = Vector2.zero;
        title.color = target;
    }
}
