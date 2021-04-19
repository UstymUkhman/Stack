using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CameraAnimation : UnityEvent<int, bool> {}

public class StackManager : MonoBehaviour
{
    [SerializeField] private CameraAnimation cameraAnimation = new CameraAnimation();
    private List<GameObject> platforms = new List<GameObject>();
    [SerializeField] private float tollerance = 0.025f;

    [SerializeField] private GameObject staticPrefab;
    [SerializeField] private GameObject dynamicPrefab;
    [SerializeField] private GameObject cuttedPrefab;

    private DynamicPlatform dynamicPlatformManager;
    private float platformHeight;

    private bool isLeft {
        get {
            return System.Convert.ToBoolean(Platforms % 2);
        }
    }

    private int Platforms {
        get {
            return platforms.Count;
        }
    }

    void Awake()
    {
        GameObject firstPlatform = transform.GetChild(0).gameObject;
        platformHeight = firstPlatform.transform.localScale.y;

        platforms.Add(firstPlatform);
        cameraAnimation.Invoke(Platforms, false);
    }

    void Start()
    {
        StartCoroutine(InitializeStack());
    }

    private IEnumerator InitializeStack()
    {
        yield return new WaitForSeconds(1.0f);
        SpawnDynamicPlatform();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dynamicPlatformManager)
        {
            dynamicPlatformManager.Stop();
            CalculatePlatformDistance();
        }
    }

    private void CalculatePlatformDistance()
    {
        bool left = !isLeft;

        GameObject staticPlatform = platforms[Platforms - 2];
        GameObject dynamicPlatform = platforms[Platforms - 1];

        Vector3 dynamicScale = dynamicPlatform.transform.localScale;
        Vector3 distanceVector = dynamicPlatform.transform.position - staticPlatform.transform.position;

        float distance = Mathf.Abs(left ? distanceVector.x : distanceVector.z);

        float allowedDistance = (
            (left ? dynamicScale.x : dynamicScale.z) +
            (left ? staticPlatform.transform.localScale.x : staticPlatform.transform.localScale.z)
        ) / 2.0f;

        if (distance <= allowedDistance)
        {
            float width = dynamicScale.x - (left ? distance : 0.0f);
            float depth = dynamicScale.z - (left ? 0.0f : distance);

            float discartedWidth = left ? dynamicScale.x - width : width;
            float discartedDepth = left ? depth : dynamicScale.z - depth;

            if ((left ? discartedWidth : discartedDepth) > tollerance)
            {
                CalculateCuttedPlatform(
                    dynamicPlatform.transform.position,
                    staticPlatform.transform.position,
                    left ? width / 2.0f : depth / 2.0f,
                    discartedWidth, discartedDepth, left
                );
            }
            else
            {
                Debug.Log("Perfect Timing!");
                width = dynamicScale.x;
                depth = dynamicScale.z;
            }

            float offset = CalculateStaticPlatform(
                staticPlatform.transform,
                dynamicPlatform.transform.position,
                width, depth, left
            );

            DestroyDynamicPlatform(dynamicPlatform);
            SpawnDynamicPlatform(width, depth, offset);
        }
        else
        {
            ConvertDynamicPlatform(dynamicPlatform);
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

    private void CalculateCuttedPlatform(Vector3 dynamicPosition, Vector3 staticPosition, float detachment, float width, float depth, bool left)
    {
        float offset = (left ? detachment + width : detachment + depth) *
            GetPlatfromOffset(staticPosition, dynamicPosition, -2.0f, 1.0f);

        Vector3 position = new Vector3(
            left ? staticPosition.x + offset : staticPosition.x,
            dynamicPosition.y,
            left ? staticPosition.z : staticPosition.z + offset
        );

        SpawnCuttedPlatform(position, width, depth);
    }

    private float GetPlatfromOffset(Vector3 staticPosition, Vector3 dynamicPosition, float range, float clamp) =>
        (float) System.Convert.ToInt32(
            dynamicPosition.x < staticPosition.x ||
            dynamicPosition.z < staticPosition.z
        ) * range + clamp;

    private void SpawnStaticPlatform(Vector3 position = default, float width = 1.2f, float depth = 1.2f)
    {
        GameObject platform = Instantiate(staticPrefab, position, Quaternion.identity, transform);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);

        cameraAnimation.Invoke(Platforms, false);
        platforms.Add(platform);
    }

    private void SpawnDynamicPlatform(float width = 1.2f, float depth = 1.2f, float offset = 0.0f)
    {
        GameObject platform = Instantiate(dynamicPrefab, Vector3.zero, Quaternion.identity, transform);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);

        dynamicPlatformManager = platform.GetComponent<DynamicPlatform>();
        dynamicPlatformManager.y = Platforms * platformHeight;
        dynamicPlatformManager.SetDirection(isLeft);
        dynamicPlatformManager.offset = offset;
        dynamicPlatformManager.Move();

        platforms.Add(platform);
    }

    private void SpawnCuttedPlatform(Vector3 position, float width, float depth)
    {
        GameObject platform = Instantiate(cuttedPrefab, position, Quaternion.identity, transform);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);
    }

    private void ConvertDynamicPlatform(GameObject dynamicPlatform)
    {
        Vector3 position = dynamicPlatform.transform.position;

        float width = dynamicPlatform.transform.localScale.x;
        float depth = dynamicPlatform.transform.localScale.z;

        SpawnCuttedPlatform(position, width, depth);
        DestroyDynamicPlatform(dynamicPlatform);
    }

    private void DestroyDynamicPlatform(GameObject dynamicPlatform)
    {
        platforms.Remove(dynamicPlatform);
        Destroy(dynamicPlatform);
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        cameraAnimation.Invoke(Platforms, true);

        /*
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        */
    }
}
