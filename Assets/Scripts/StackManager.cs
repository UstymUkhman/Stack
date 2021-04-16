using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [SerializeField] private GameObject staticPrefab;
    [SerializeField] private GameObject dynamicPrefab;
    // [SerializeField] private GameObject cuttedPrefab;

    List<GameObject> platforms = new List<GameObject>();

    private DynamicPlatform dynamicPlatformManager;
    private const float platformHeight = 0.1f;

    private bool isLeft {
        get {
            return System.Convert.ToBoolean(platforms.Count % 2);
        }
    }

    void Start()
    {
        SpawnStaticPlatform();
        SpawnDynamicPlatform();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dynamicPlatformManager.Stop();
            CalculatePlatformDistance();
        }
    }

    private void SpawnStaticPlatform(Vector3 position = default, float width = 1.0f, float depth = 1.0f)
    {
        GameObject platform = Instantiate(staticPrefab, position, Quaternion.identity, transform);
        platform.transform.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);
        platforms.Add(platform);
    }

    private void SpawnDynamicPlatform(float width = 1.0f, float depth = 1.0f)
    {
        GameObject platform = Instantiate(dynamicPrefab, Vector3.zero, Quaternion.identity, transform);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);

        dynamicPlatformManager = platform.GetComponent<DynamicPlatform>();
        dynamicPlatformManager.Y = platforms.Count * platformHeight;
        dynamicPlatformManager.SetDirection(isLeft);
        dynamicPlatformManager.Move();

        platforms.Add(platform);
    }

    private void SpawnCuttedPlatform(Vector3 position, float width, float depth)
    {
        GameObject platform = Instantiate(staticPrefab, position, Quaternion.identity, transform);
        platform.transform.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);
    }

    private void CalculatePlatformDistance()
    {
        bool left = !isLeft;

        GameObject staticPlatform = platforms[platforms.Count - 2];
        GameObject dynamicPlatform = platforms[platforms.Count - 1];

        Vector3 distanceVector = dynamicPlatform.transform.position - staticPlatform.transform.position;

        float distance = Mathf.Abs(left ? distanceVector.x : distanceVector.z);

        float allowedDistance = (
            (left ? dynamicPlatform.transform.localScale.x : dynamicPlatform.transform.localScale.z) +
            (left ? staticPlatform.transform.localScale.x : staticPlatform.transform.localScale.z)
        ) / 2.0f;

        if (distance <= allowedDistance)
        {
            float width = dynamicPlatform.transform.localScale.x - (left ? distance : 0.0f);
            float depth = dynamicPlatform.transform.localScale.z - (left ? 0.0f : distance);

            float offset = CalculateStaticPlatform(
                staticPlatform.transform,
                dynamicPlatform.transform.position,
                width, depth, left
            );

            CalculateCuttedPlatform(
                dynamicPlatform.transform,
                staticPlatform.transform.position,
                width, depth, left
            );

            platforms.Remove(dynamicPlatform);
            Destroy(dynamicPlatform);

            SpawnDynamicPlatform(width, depth);
            dynamicPlatformManager.Offset = offset;
        }
        else
        {
            GameOver();
        }
    }

    private float CalculateStaticPlatform(Transform staticTransform, Vector3 dynamicPosition, float width, float depth, bool left)
    {
        float offset = (left ? staticTransform.localScale.x - width : staticTransform.localScale.z - depth) /
            GetPlatfromOffset(staticTransform.position, dynamicPosition, -4.0f, 2.0f);

        Vector3 position = new Vector3(
            left ? staticTransform.position.x + offset : staticTransform.position.x,
            dynamicPosition.y,
            left ? staticTransform.position.z : staticTransform.position.z + offset
        );

        SpawnStaticPlatform(position, width, depth);
        return left ? position.x : position.z;
    }

    private void CalculateCuttedPlatform(Transform dynamicTransform, Vector3 staticPosition, float width, float depth, bool left)
    {
        float discartedWidth = left ? dynamicTransform.localScale.x - width : width;
        float discartedDepth = left ? depth : dynamicTransform.localScale.z - depth;

        float offset = (left ? width / 2.0f + discartedWidth : depth / 2.0f + discartedDepth) *
            GetPlatfromOffset(staticPosition, dynamicTransform.position, -2.0f, 1.0f);

        Vector3 position = new Vector3(
            left ? staticPosition.x + offset : staticPosition.x,
            dynamicTransform.position.y,
            left ? staticPosition.z : staticPosition.z + offset
        );

        SpawnCuttedPlatform(position, discartedWidth, discartedDepth);
    }

    private float GetPlatfromOffset(Vector3 staticPosition, Vector3 dynamicPosition, float range, float clamp) =>
        (float) System.Convert.ToInt32(
            dynamicPosition.x < staticPosition.x ||
            dynamicPosition.z < staticPosition.z
        ) * range + clamp;

    private void GameOver()
    {
        Debug.Log("Game Over!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
