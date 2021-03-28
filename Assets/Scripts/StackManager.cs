using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [SerializeField]
    private GameObject platformPrefab;
    public static StackManager instance;

    private PlatformCutter platformCutter;
    private PlatfomManager platfomManager;

    private GameObject lastPlatform;
    private bool collided = false;
    private int platforms = 0;

    void Awake()
    {
        platformCutter = GetComponent<PlatformCutter>();

        if (instance is null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Spawn();
    }

    public void Stop()
    {
        platfomManager.Stop();
    }

    public void Spawn()
    {
        bool moveLeft = !Convert.ToBoolean(platforms % 2);

        Vector3 position = moveLeft
            ? new Vector3(0, 1, 1.5f)
            : new Vector3(-1.5f, 1, 0);

        lastPlatform = Instantiate(platformPrefab, position, Quaternion.identity, transform);
        platfomManager = lastPlatform.GetComponent<PlatfomManager>();
        platfomManager.SetDirection(moveLeft);

        /*position.x = 0f;
        position.y = ++platforms * 0.1f;
        position.z = 0f;

        transform.position = position;*/
        platforms++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collided) return;

        bool moveLeft = Convert.ToBoolean(platforms % 2);

        float offset = !moveLeft
            ? collision.transform.localPosition.z
            : collision.transform.localPosition.x;

        if (offset == 0)
        {
            Debug.Log("Perfect!");
            return;
        }

        int half = Math.Sign(offset) * 2;

        if (!moveLeft)
        {
            float x = transform.position.x + transform.localScale.x / half;
            collided = platformCutter.CutVertically(collision.transform, x);
        }
        else
        {
            float z = transform.position.z + transform.localScale.z / -half;
            collided = platformCutter.CutHorizontally(collision.transform, z);
        }
    }
}
