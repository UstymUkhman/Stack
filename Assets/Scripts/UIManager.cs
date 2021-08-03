using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private bool visibleStart;
    private bool visibleRestart;

    private RawImage background;
    private CanvasGroup canvas;
    private RawImage shadow;

    [SerializeField] private Text ctaStart;
    [SerializeField] private Text ctaRestart;

    [SerializeField] private TitleManager title;
    [SerializeField] private ScoreManager score;

    [Header("On \"Tap To Start\" game event:")]
    [SerializeField] private UnityEvent start = new UnityEvent();

    [Header("On \"Tap To Restart\" game event:")]
    [SerializeField] private UnityEvent restart = new UnityEvent();

    private void Awake()
    {
        background = transform.GetChild(0).GetComponent<RawImage>();
        shadow = transform.GetChild(1).GetComponent<RawImage>();
        canvas = transform.GetComponent<CanvasGroup>();

        ctaRestart.color = ColorManager.TRANSPARENT;
        ctaStart.color = ColorManager.TRANSPARENT;

        Texture2D texture = new Texture2D(1, 2);

        texture.SetPixels(new Color[] {
            ColorManager.SOLID_BLACK,
            ColorManager.TRANSPARENT
        });

        texture.Apply();
        shadow.texture = texture;

        shadow.texture.wrapMode = TextureWrapMode.Clamp;
        shadow.texture.filterMode = FilterMode.Bilinear;
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
        shadow.CrossFadeAlpha(0.0f, 0.0f, true);
        StartCoroutine(ShowStart(true));
    }

    private void Update()
    {
        if (canvas.interactable && Input.GetMouseButtonDown(0))
        {
            if (visibleStart)
            {
                StartCoroutine(HideStart());
            }

            else if (visibleRestart)
            {
                HideRestart();
            }
        }
    }

    private IEnumerator ShowStart(bool initial = false)
    {
        yield return new WaitForSeconds(
            1.0f + System.Convert.ToInt32(!initial) * 0.5f
        );

        StartCoroutine(FadeCTA(ctaStart));
        if (!initial) title.FadeIn();

        canvas.interactable = true;
        visibleStart = true;
    }

    public void OnGameOver()
    {
        StartCoroutine(ShowRestart());
    }

    private IEnumerator ShowRestart()
    {
        yield return new WaitForSeconds(0.5f);

        shadow.CrossFadeAlpha(1.0f, 0.5f, false);
        StartCoroutine(FadeCTA(ctaRestart));
        score.ShowBest();

        yield return new WaitForSeconds(0.5f);
        canvas.interactable = true;
        visibleRestart = true;
    }

    private IEnumerator HideStart()
    {
        title.FadeOut();
        visibleStart = false;
        canvas.interactable = false;

        StartCoroutine(FadeCTA(ctaStart, false));
        yield return new WaitForSeconds(1.0f);

        score.ShowCurrent();
        start.Invoke();
    }

    private void HideRestart()
    {
        visibleRestart = false;
        canvas.interactable = false;

        StartCoroutine(FadeCTA(ctaRestart, false));
        shadow.CrossFadeAlpha(0.0f, 0.5f, false);

        StartCoroutine(ShowStart(false));
        restart.Invoke();
        score.Hide();
    }

    private IEnumerator FadeCTA(Text cta, bool visible = true, float duration = 0.5f)
    {
        float startTime = Time.time;
        float endTime = Time.time + duration;

        Color32 target = visible ? ColorManager.SOLID_WHITE : ColorManager.TRANSPARENT;
        Color32 current = visible ? ColorManager.TRANSPARENT : ColorManager.SOLID_WHITE;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / duration;
            cta.color = Color32.Lerp(current, target, time);
            yield return null;
        }

        cta.color = target;
    }
}
