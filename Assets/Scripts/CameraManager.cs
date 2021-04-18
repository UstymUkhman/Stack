using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float followUp = 0.5f;
    [SerializeField] private float zoomOut = 1.0f;

    public void CheckCameraAnimation(int platforms, bool gameOver)
    {
        if (platforms > 3) MoveUp();
        if (gameOver) ZoomOut();
    }

    private void MoveUp()
    {
        StartCoroutine(Animate(new Vector3(
            transform.position.x, transform.position.y + 0.1f, transform.position.z
        ), followUp));
    }

    private void ZoomOut()
    {
        StartCoroutine(Animate(new Vector3(
            // transform.position.x, transform.position.y, transform.position.z
        ), zoomOut));
    }

    private IEnumerator Animate(Vector3 target, float duration)
    {
        float startTime = Time.time;
        float endTime = Time.time + duration;
        Vector3 position = transform.position;

        while (Time.time < endTime)
        {
            transform.position = Vector3.Lerp(position, target, (Time.time - startTime) / duration);
            yield return null;
        }

        transform.position = target;
    }
}
