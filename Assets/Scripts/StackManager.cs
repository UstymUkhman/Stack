using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [SerializeField]
    private GameObject staticPrefab;

    [SerializeField]
    private GameObject dynamicPrefab;

    private DynamicPlatform dynamicPlatform;
    private StaticPlatform staticPlatform;

    private GameObject lastPlatform;
    private int platforms = 0;

    void Start()
    {
        SpawnStatic();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dynamicPlatform.Stop();
            // Spawn();
        }
    }

    private void SpawnStatic()
    {
        GameObject platform = Instantiate(staticPrefab, Vector3.zero, Quaternion.identity, transform);
        staticPlatform = platform.GetComponent<StaticPlatform>();
        SpawnDynamic();
    }

    private void SpawnDynamic()
    {
        bool moveLeft = !Convert.ToBoolean(platforms % 2);

        Vector3 position = moveLeft
            ? new Vector3(0, 1, 1.5f)
            : new Vector3(-1.5f, 1, 0);

        lastPlatform = Instantiate(dynamicPrefab, position, Quaternion.identity, transform);
        dynamicPlatform = lastPlatform.GetComponent<DynamicPlatform>();

        dynamicPlatform.SetDirection(moveLeft);
        staticPlatform.index = platforms;

        /*position.x = 0f;
        position.y = ++platforms * 0.1f;
        position.z = 0f;

        transform.position = position;*/
        platforms++;
    }
}
