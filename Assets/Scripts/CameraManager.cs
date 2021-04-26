using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [Header("\"Move Up\" and \"Zoom Out\" animations duration (in seconds):")]
    [SerializeField] private float animationDuration = 0.5f;

    private Vector3 initialPosition;
    private new Camera camera;

    void Awake()
    {
        camera = Camera.main;
        initialPosition = transform.position;
    }

    public void CheckCameraAnimation(int platforms, bool gameOver)
    {
        float zoom = Mathf.Min(Mathf.Floor(platforms / 15.0f), 4.0f);

        if (gameOver && zoom > 0.0f) ZoomOut(zoom);
        else if (platforms > 3) MoveUp();
    }

    private void MoveUp()
    {
        StartCoroutine(AnimateVertically(transform.position.y + 0.2f));
    }

    private void ZoomOut(float zoomLevel)
    {
        float scale = zoomLevel * 3.0f;
        float size = scale - (zoomLevel * 0.5f + 1.0f);

        StartCoroutine(AnimateSize(camera.orthographicSize + size));
        StartCoroutine(AnimateVertically(initialPosition.y + scale));
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
