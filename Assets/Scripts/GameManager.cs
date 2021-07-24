using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Title Fade Out animation event:")]
    [SerializeField] private UnityEvent titleDispose = new UnityEvent();

    private static GameManager instance;
    private StackManager stack;

    private bool gameOver = true;
    private bool started = false;

    public static GameManager Instance {
        get {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null || instance == this)
        {
            stack = GetComponent<StackManager>();
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        ColorManager.SetPlatformColors(0);
        stack.CreateFirstPlatform();

        yield return new WaitForSeconds(5.0f);
        gameOver = false;
    }

    private void Update()
    {
        if (gameOver && !started)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnScreenTap();
        }
    }

    private void OnScreenTap()
    {
        if (!started)
        {
            stack.SpawnDynamicPlatform();
            titleDispose.Invoke();
            started = true;
            return;
        }

        if (!gameOver)
        {
            gameOver = stack.StopDynamicPlatform();
        }
        else
        {
            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        stack.Explode();
        yield return new WaitForSeconds(1.0f);

        stack.Reset();
        started = false;
        StartCoroutine(Initialize());
    }

    private void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }
}
