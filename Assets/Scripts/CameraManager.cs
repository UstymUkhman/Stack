using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [Header("\"Move Up\", \"Zoom Out\" and \"Reset\" animations duration (in seconds):")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("Camera starts following the stack after this platform:")]
    [SerializeField] private int followPlatform = 3;

    private float initialHeight;
    private float initialSize;

#pragma warning disable CS0108
    private Camera camera;
#pragma warning restore CS0108

    private void Awake()
    {
        camera = Camera.main;
        initialHeight = transform.position.y;
        initialSize = camera.orthographicSize;
    }

    public void CheckCameraAnimation(int platforms, bool gameOver, bool explode)
    {
        float zoom = Mathf.Min(Mathf.Floor(platforms / 15.0f), 4.0f);

        if (gameOver && platforms == 0)
        {
            transform.position = new Vector3(
                transform.position.x,
                initialHeight,
                transform.position.z
            );
        }
        else if (explode)
        {
            float offset = zoom * 2.5f + 5.0f;
            float target = transform.position.y + offset;

            StartCoroutine(AnimateVertically(target));
            StartCoroutine(AnimateSize(initialSize));
        }
        else if (!gameOver && platforms > followPlatform)
        {
            float offset = (platforms - followPlatform) * 0.2f;
            StartCoroutine(AnimateVertically(initialHeight + offset));
        }
        else if (gameOver && zoom > 0.0f)
        {
            ZoomOut(zoom);
        }
    }

    private void ZoomOut(float zoomLevel)
    {
        float scale = zoomLevel * 3.0f;
        float size = scale - (zoomLevel * 0.5f + 1.0f);

        StartCoroutine(AnimateVertically(initialHeight + scale));
        StartCoroutine(AnimateSize(camera.orthographicSize + size));
    }

    private IEnumerator AnimateVertically(float y)
    {
        float startTime = Time.time;
        float endTime = Time.time + animationDuration;

        Vector3 position = transform.position;
        Vector3 target = new Vector3(position.x, y, position.z);

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / animationDuration;
            transform.position = Vector3.Lerp(position, target, time);
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator AnimateSize(float size)
    {
        float startTime = Time.time;
        float currentSize = camera.orthographicSize;
        float endTime = Time.time + animationDuration;

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / animationDuration;
            camera.orthographicSize = Mathf.Lerp(currentSize, size, time);
            yield return null;
        }

        camera.orthographicSize = size;
    }
}
