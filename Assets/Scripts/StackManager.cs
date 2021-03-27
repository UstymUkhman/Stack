using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [SerializeField]
    private GameObject platformPrefab;
    public static StackManager instance;

    private Vector3 position = new Vector3();
    private Quaternion rotation = Quaternion.Euler(0, 45, 0);

    private PlatfomManager platfomManager;
    private GameObject lastPlatform;
    private int platforms = 0;

    void Awake()
    {
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
        position.x = (platforms++ % 2) * -2 + 1;
        position.y = 0f;
        position.z = 1f;

        lastPlatform = Instantiate(platformPrefab, position, rotation, transform);

        platfomManager = lastPlatform.GetComponent<PlatfomManager>();
        platfomManager.SetDirection(Convert.ToBoolean(platforms % 2));

        position.x = 0f;
        position.y = platforms * 0.1f;
        position.z = 0f;

        transform.position = position;
    }
}
