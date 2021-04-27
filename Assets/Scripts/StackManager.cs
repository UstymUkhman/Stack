using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CameraAnimation : UnityEvent<int, bool> { }

public class StackManager : MonoBehaviour
{
    [Header("Camera animation events for \"Move Up\" and \"Zoom Out\":")]
    [SerializeField] private CameraAnimation cameraAnimation = new CameraAnimation();

    private List<GameObject> platforms = new List<GameObject>();

    [Header("Offset to ignore for a perfect timing:")]
    [SerializeField] private float tollerance = 0.025f;

    [SerializeField] private GameObject staticPrefab;
    [SerializeField] private GameObject dynamicPrefab;
    [SerializeField] private GameObject cuttedPrefab;
    [SerializeField] private GameObject planePrefab;

    private DynamicPlatform dynamicPlatformManager;
    private int perfectPlatformsCount = 0;
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
        CreateFirstPlatform();
    }

    void Start()
    {
        SetPlatformColor();
        StartCoroutine(InitializeStack());
    }

    private void CreateFirstPlatform()
    {
        GameObject platform = transform.GetChild(0).gameObject;
        platformHeight = platform.transform.localScale.y;

        platforms.Add(platform);
        cameraAnimation.Invoke(Platforms, false);
    }

    private IEnumerator InitializeStack()
    {
        yield return new WaitForSeconds(1.0f);
        SpawnDynamicPlatform();
    }

    private void SetPlatformColor(int last = 1)
    {
        Color platformColor = ColorManager.Instance.GetPlatformColor(Platforms - last);
        platforms[Platforms - 1].GetComponent<MeshRenderer>().material.SetColor("_Color", platformColor);
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

            bool perfect = (left ? discartedWidth : discartedDepth) <= tollerance;

            if (!perfect)
            {
                perfectPlatformsCount = 0;

                CalculateCuttedPlatform(
                    dynamicPlatform.transform.position,
                    staticPlatform.transform.position,
                    left ? width / 2.0f : depth / 2.0f,
                    discartedWidth, discartedDepth
                );
            }
            else
            {
                perfectPlatformsCount++;
                width = dynamicScale.x;
                depth = dynamicScale.z;
            }

            float offset = CalculateStaticPlatform(
                dynamicPlatform.transform.position,
                staticPlatform.transform,
                width, depth
            );

            StartCoroutine(SpawnPlanes(perfect));
            DestroyDynamicPlatform(dynamicPlatform);
            SpawnDynamicPlatform(width, depth, offset);
        }
        else
        {
            ConvertDynamicPlatform(dynamicPlatform);
            GameOver();
        }
    }

    private void CalculateCuttedPlatform(Vector3 dynamicPosition, Vector3 staticPosition, float detachment, float width, float depth)
    {
        float offset = (isLeft ? detachment + depth : detachment + width) *
            GetPlatfromOffset(staticPosition, dynamicPosition, -2.0f, 1.0f);

        Vector3 position = new Vector3(
            isLeft ? staticPosition.x : staticPosition.x + offset,
            dynamicPosition.y,
            isLeft ? staticPosition.z + offset : staticPosition.z
        );

        SpawnCuttedPlatform(position, width, depth);
    }

    private float CalculateStaticPlatform(Vector3 dynamicPosition, Transform staticTransform, float width, float depth)
    {
        float offset = (isLeft ? staticTransform.localScale.z - depth : staticTransform.localScale.x - width) /
            GetPlatfromOffset(staticTransform.position, dynamicPosition, -4.0f, 2.0f);

        Vector3 position = new Vector3(
            isLeft ? staticTransform.position.x : staticTransform.position.x + offset,
            dynamicPosition.y,
            isLeft ? staticTransform.position.z + offset : staticTransform.position.z
        );

        SpawnStaticPlatform(position, width, depth);
        return isLeft ? position.x : position.z;
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
        SetPlatformColor(2);
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
        SetPlatformColor();
    }

    private void SpawnCuttedPlatform(Vector3 position, float width, float depth)
    {
        GameObject platform = Instantiate(cuttedPrefab, position, Quaternion.identity, transform);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);

        Color platformColor = ColorManager.Instance.GetPlatformColor(Platforms - 1);
        platform.GetComponent<MeshRenderer>().material.SetColor("_Color", platformColor);
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

    private void SpawnPlane(Transform staticTransform, int index)
    {
        Vector3 scale = staticTransform.localScale * 0.1f;
        Vector3 position = staticTransform.position;
        position.y -= platformHeight / 2.0f;

        GameObject plane = Instantiate(planePrefab, position, Quaternion.identity, transform);
        plane.transform.localScale = new Vector3(scale.x, 1.0f, scale.z);

        if (index > 0)
        {
            PlatformPlane platformPlane = plane.GetComponent<PlatformPlane>();
            StartCoroutine(platformPlane.AnimateScale());
            platformPlane.AddRenderQueue(index);
        }
    }

    private IEnumerator SpawnPlanes(bool matched)
    {
        Transform staticTransform = platforms[Platforms - 1].transform;
        int animatedPlanes = Mathf.Min(perfectPlatformsCount - 3, 3);

        if (matched) SpawnPlane(staticTransform, 0);

        for (int p = 1; p <= animatedPlanes; p++)
        {
            yield return new WaitForSeconds(p * 0.1f);
            SpawnPlane(staticTransform, p);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        cameraAnimation.Invoke(Platforms, true);
    }
}
