using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Title fade out animation event:")]
    [SerializeField] private UnityEvent titleDispose = new UnityEvent();

    [Header("\"Tap To Start\" fade in animation event:")]
    [SerializeField] private UnityEvent tapToStart = new UnityEvent();

    [Header("\"Tap To Restart\" fade in animation event:")]
    [SerializeField] private UnityEvent tapToRestart = new UnityEvent();

    private static GameManager instance;
    private StackManager stack;

    private bool gameStart = true;
    private bool gameOver = true;

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
        float startDelay = gameStart ? 3.0f : 1.0f;

        StartCoroutine(stack.CreateFirstPlatform(startDelay));
        yield return new WaitForSeconds(startDelay + 1.0f);

        if (gameStart) tapToStart.Invoke();
    }

    private void Update()
    {
        if (gameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            gameOver = stack.StopDynamicPlatform();

            if (gameOver)
            {
                tapToRestart.Invoke();
            }
        }
    }

    public void OnStart()
    {
        stack.SpawnDynamicPlatform();
        titleDispose.Invoke();

        gameStart = false;
        gameOver = false;
    }

    public void OnRestart()
    {
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        stack.Explode();
        yield return new WaitForSeconds(1.0f);

        stack.Reset();
        StartCoroutine(Initialize());

        yield return new WaitForSeconds(2.0f);
        OnStart();
    }

    private void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }
}
