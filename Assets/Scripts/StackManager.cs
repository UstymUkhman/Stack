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

            Vector3 position = GetNewPlatformPosition(
                staticPlatform.transform,
                dynamicPlatform.transform.position,
                width, depth, left
            );

            SpawnStaticPlatform(position, width, depth);
            platforms.Remove(dynamicPlatform);
            Destroy(dynamicPlatform);

            SpawnDynamicPlatform(width, depth);
            dynamicPlatformManager.Offset = left ? position.x : position.z;
        }
        else
        {
            GameOver();
        }
    }

    private Vector3 GetNewPlatformPosition(Transform staticTransform, Vector3 position, float width, float depth, bool left)
    {
        float px = staticTransform.position.x;
        float pz = staticTransform.position.z;

        float offset = (left
            ? staticTransform.localScale.x - width
            : staticTransform.localScale.z - depth
        ) / (
            (float) System.Convert.ToInt32(
                position.x < px || position.z < pz
            ) * -4.0f + 2.0f
        );

        return new Vector3(
            left ? px + offset : px,
            position.y,
            left ? pz : pz + offset
        );
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
