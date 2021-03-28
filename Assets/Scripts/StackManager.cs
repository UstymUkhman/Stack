using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [SerializeField]
    private GameObject staticPrefab;

    [SerializeField]
    private GameObject cuttedPrefab;

    [SerializeField]
    private GameObject dynamicPrefab;

    private DynamicPlatform dynamicPlatform;
    private StaticPlatform staticPlatform;

    private GameObject lastPlatform;
    private int platforms = 0;

    void Start()
    {
        GameObject platform = Instantiate(staticPrefab, Vector3.zero, Quaternion.identity, transform);
        staticPlatform = platform.GetComponent<StaticPlatform>();

        staticPlatform.cuttedPrefab = cuttedPrefab;
        staticPlatform.index = platforms;
        SpawnDynamicPlatform();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dynamicPlatform.Stop();
            Invoke("SpawnNewPlatform", 1.0f);
        }
    }

    private void SpawnNewPlatform()
    {
        Transform platform = transform.Find("Platform" + platforms);

        SpawnDynamicPlatform();

        if (platform != null)
        {
            staticPlatform = platform.GetComponent<StaticPlatform>();
            lastPlatform.transform.localScale = platform.localScale;

            dynamicPlatform.offset = Convert.ToBoolean(platforms % 2)
                ? platform.localPosition.x : platform.localPosition.z;

            staticPlatform.cuttedPrefab = cuttedPrefab;
            staticPlatform.index = platforms - 1;
        }
    }

    private void SpawnDynamicPlatform()
    {
        int nextPlatform = platforms + 1;
        bool moveLeft = !Convert.ToBoolean(platforms % 2);

        Vector3 position = moveLeft
            ? new Vector3(0, 1, 1.5f)
            : new Vector3(-1.5f, 1, 0);

        lastPlatform = Instantiate(dynamicPrefab, position, Quaternion.identity, transform);
        dynamicPlatform = lastPlatform.GetComponent<DynamicPlatform>();

        dynamicPlatform.SetDirection(moveLeft);
        dynamicPlatform.index = nextPlatform;

        platforms = nextPlatform;
    }
}
