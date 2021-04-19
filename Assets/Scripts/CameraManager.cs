using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.5f;
    private const float platformHeight = 0.2f;

    private Vector3 initialPosition;
    private new Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
        initialPosition = transform.position;
    }

    public void CheckCameraAnimation(int platforms, bool gameOver)
    {
        float zoom = Mathf.Floor(platforms / 15.0f);

        if (gameOver && zoom > 0.0f) ZoomOut(zoom);
        else if (platforms > 3) MoveUp();
    }

    private void MoveUp()
    {
        StartCoroutine(AnimateVertically(
            transform.position.y + platformHeight
        ));
    }

    private void ZoomOut(float zoomLevel)
    {
        float size = zoomLevel;
        float scale = size + 1;

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
