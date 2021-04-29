using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CameraAnimation : UnityEvent<int, bool, bool> { }

public class StackManager : MonoBehaviour
{
    [Header("Camera animation events for \"Move Up\", \"Zoom Out\" and \"Reset\":")]
    [SerializeField] private CameraAnimation cameraAnimation = new CameraAnimation();

    private List<GameObject> platforms = new List<GameObject>();

    [Header("Offset to ignore for a perfect timing:")]
    [SerializeField] private float tollerance = 0.025f;

    [SerializeField] private GameObject dynamicPrefab;
    [SerializeField] private GameObject staticPrefab;
    [SerializeField] private GameObject cuttedPrefab;
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private GameObject planePrefab;

    private Dynamic dynamicPlatformManager;
    private int perfectPlatformsCount = 0;
    private float platformHeight = 0.0f;

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

    public void CreateFirstPlatform()
    {
        GameObject platform = transform.GetChild(0).gameObject;
        platformHeight = platform.transform.localScale.y;

        platforms.Add(platform);
        SetPlatformColor();
    }

    private void SetPlatformColor(int last = 1)
    {
        Color platformColor = ColorManager.GetPlatformColor(Platforms - last);
        platforms[Platforms - 1].GetComponent<MeshRenderer>().material.SetColor("_Color", platformColor);
    }

    public bool StopDynamicPlatform()
    {
        dynamicPlatformManager.Stop();
        return CalculatePlatformDistance();
    }

    private bool CalculatePlatformDistance()
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
            cameraAnimation.Invoke(Platforms, true, false);
            ConvertDynamicPlatform(dynamicPlatform);
            return true;
        }

        return false;
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
        Vector3 scale = new Vector3(width, platformHeight, depth);

        cameraAnimation.Invoke(Platforms, false, false);
        platform.transform.localScale = scale;

        platforms.Add(platform);
        SetPlatformColor(2);
    }

    public void SpawnDynamicPlatform(float width = 1.2f, float depth = 1.2f, float offset = 0.0f)
    {
        GameObject platform = Instantiate(dynamicPrefab, Vector3.zero, Quaternion.identity, transform);
        platform.transform.localScale = new Vector3(width, platformHeight, depth);

        dynamicPlatformManager = platform.GetComponent<Dynamic>();
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

        Color platformColor = ColorManager.GetPlatformColor(Platforms - 1);
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
            Plane platformPlane = plane.GetComponent<Plane>();
            StartCoroutine(platformPlane.AnimateScale());
            platformPlane.AddRenderQueue(index);
        }
    }

    private IEnumerator SpawnPlanes(bool matched)
    {
        Transform staticTransform = platforms[Platforms - 1].transform;
        int animatedPlanes = Mathf.Min(perfectPlatformsCount - 3, 5);

        if (matched)
        {
            SpawnPlane(staticTransform, 0);
        }

        for (int p = 1; p <= animatedPlanes; p++)
        {
            yield return new WaitForSeconds(p * 0.05f);
            SpawnPlane(staticTransform, p);
        }
    }

    public void Explode()
    {
        int firstPlatform = Mathf.Max(transform.childCount - 6, 0);

        for (int p = transform.childCount - 1; p > firstPlatform; p--)
        {
            GameObject platform = transform.GetChild(p).gameObject;

            if (platform.GetComponent<Rigidbody>() == null)
            {
                platform.AddComponent<Rigidbody>().mass = 10.0f;
            }
        }

        Vector3 position = new Vector3(0.1f, Platforms * platformHeight - 1.0f, 0.1f);
        Instantiate(spherePrefab, position, Quaternion.identity, transform);
        cameraAnimation.Invoke(Platforms, true, true);
    }

    public void Reset()
    {
        for (int c = 1; c < transform.childCount; c++)
        {
            Destroy(transform.GetChild(c).gameObject);
        }

        cameraAnimation.Invoke(0, true, false);
        platforms.Clear();
    }
}
