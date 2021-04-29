using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
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
            ColorManager.SetPlatformColors(0);
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
        stack.CreateFirstPlatform();
        yield return new WaitForSeconds(1.0f);
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
            started = true;
            return;
        }

        if (!gameOver)
        {
            gameOver = stack.StopDynamicPlatform();
        }
        else
        {
            Reset();
        }
    }

    private void Reset()
    {
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
