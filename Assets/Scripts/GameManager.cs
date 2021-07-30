using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("On \"Game Over\" UI animation event:")]
    [SerializeField] private UnityEvent onGameOver = new UnityEvent();

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
        Initialize();
    }

    private void Initialize()
    {
        ColorManager.SetPlatformColors(0);

        StartCoroutine(stack.CreateFirstPlatform(
            System.Convert.ToInt32(gameStart) * 3.0f
        ));
    }

    private void Update()
    {
        if (gameOver) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            gameOver = stack.StopDynamicPlatform();

            if (gameOver)
            {
                onGameOver.Invoke();
            }
        }
    }

    public void OnStart()
    {
        stack.SpawnDynamicPlatform();
        gameStart = false;
        gameOver = false;
    }

    public void OnRestart()
    {
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        StartCoroutine(stack.Explode());
        yield return new WaitForSeconds(1.0f);

        stack.Reset();
        Initialize();
    }

    private void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }
}
