using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private bool visibleStart;
    private bool visibleRestart;

    private CanvasGroup canvas;

    [SerializeField] private TextMeshProUGUI ctaStart;
    [SerializeField] private TextMeshProUGUI ctaRestart;

    [Header("On \"Tap To Start\" game event:")]
    [SerializeField] private UnityEvent start = new UnityEvent();

    [Header("On \"Tap To Restart\" game event:")]
    [SerializeField] private UnityEvent restart = new UnityEvent();

    private void Awake()
    {
        canvas = transform.GetComponent<CanvasGroup>();
        ctaRestart.color = ColorManager.TRANSPARENT;
        ctaStart.color = ColorManager.TRANSPARENT;
        canvas.alpha = 0.0f;
    }

    private void Update()
    {
        if (canvas.interactable && Input.GetMouseButtonDown(0))
        {
            if (visibleStart)
            {
                HideStart();
            }
            else if (visibleRestart)
            {
                HideRestart();
            }
        }
    }

    public void ShowStart()
    {
        visibleStart = true;
        ShowCTA(ctaStart);
    }

    public void HideStart()
    {
        visibleStart = false;
        start.Invoke();
        HideCTA();
    }

    public void ShowRestart()
    {
        ctaStart.color = ColorManager.TRANSPARENT;
        StartCoroutine(OnRestart());
    }

    private IEnumerator OnRestart()
    {
        yield return new WaitForSeconds(0.5f);
        visibleRestart = true;
        ShowCTA(ctaRestart);
    }

    public void HideRestart()
    {
        visibleRestart = false;
        restart.Invoke();
        HideCTA();
    }

    private void ShowCTA(TextMeshProUGUI cta)
    {
        cta.color = ColorManager.SOLID;
        StartCoroutine(FadeAlpha(0.5f));
        canvas.interactable = true;
    }

    private void HideCTA()
    {
        StartCoroutine(FadeAlpha(0.5f, false));
        canvas.interactable = false;
    }

    private IEnumerator FadeAlpha(float duration, bool visible = true)
    {
        float startTime = Time.time;
        float endTime = Time.time + duration;

        float target = visible ? 1.0f : 0.0f;
        float current = visible ? 0.0f : 1.0f;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / duration;
            canvas.alpha = Mathf.Lerp(current, target, time);
            yield return null;
        }

        canvas.alpha = target;
    }
}
