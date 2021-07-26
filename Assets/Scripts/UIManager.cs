using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private bool visibleStart;
    private bool visibleRestart;

    private CanvasGroup canvas;
    private RawImage background;

    [SerializeField] private TitleManager title;
    [SerializeField] private TextMeshProUGUI ctaStart;
    [SerializeField] private TextMeshProUGUI ctaRestart;

    [Header("On \"Tap To Start\" game event:")]
    [SerializeField] private UnityEvent start = new UnityEvent();

    [Header("On \"Tap To Restart\" game event:")]
    [SerializeField] private UnityEvent restart = new UnityEvent();

    private void Awake()
    {
        background = transform.GetComponentInChildren<RawImage>();
        canvas = transform.GetComponent<CanvasGroup>();

        ctaRestart.color = ColorManager.TRANSPARENT;
        ctaStart.color = ColorManager.TRANSPARENT;
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        StartCoroutine(title.Initialize());
        yield return new WaitForSeconds(2.5f);

        background.CrossFadeAlpha(0.0f, 1.0f, false);
        StartCoroutine(ShowStart());
    }

    private void Update()
    {
        if (canvas.interactable && Input.GetMouseButtonDown(0))
        {
            if (visibleStart)
            {
                StartCoroutine(HideStart());
            }

            /* else if (visibleRestart)
            {
                HideRestart();
            } */
        }
    }

    private IEnumerator ShowStart()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(FadeCTA(ctaStart));

        canvas.interactable = true;
        visibleStart = true;
    }

    private IEnumerator HideStart()
    {
        title.FadeOut();
        visibleStart = false;
        canvas.interactable = false;

        StartCoroutine(FadeCTA(ctaStart, false));
        yield return new WaitForSeconds(1.0f);
        start.Invoke();
    }

    private IEnumerator FadeCTA(TextMeshProUGUI cta, bool visible = true, float duration = 0.5f)
    {
        float startTime = Time.time;
        float endTime = Time.time + duration;

        Color32 target = visible ? ColorManager.SOLID : ColorManager.TRANSPARENT;
        Color32 current = visible ? ColorManager.TRANSPARENT : ColorManager.SOLID;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / duration;
            cta.color = Color32.Lerp(current, target, time);
            yield return null;
        }

        cta.color = target;
    }

    /* private void ShowCTA(TextMeshProUGUI cta)
    {
        cta.color = ColorManager.SOLID;
        
        canvas.interactable = true;
    } */

    /* public void HideStart()
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

    private void HideCTA()
    {
        StartCoroutine(FadeAlpha(0.5f, false));
        canvas.interactable = false;
    } */

    /* private IEnumerator FadeAlpha(float duration, bool visible = true)
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
    } */
}
