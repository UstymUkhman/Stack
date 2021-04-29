using UnityEngine;
using System.Collections;

public class Plane : MonoBehaviour
{
    [Header("Margin from plane border (range 0.0 - 1.0):")]
    [SerializeField] private float borderMargin = 0.1f;

    [Header("Scale multiplier for fade out animation:")]
    [SerializeField] private float scaleFactor = 1.5f;

    private const float PLANE_SIZE   = 0.12f;
    private const float HOLE_SIZE    = 10.0f;
    private const int MAX_MASK_QUEUE = 3012;

    private float animationDuration;
    private Transform holeTransform;

    private Material planeMaterial;
    private Material holeMaterial;

    private void Awake()
    {
        holeTransform = transform.GetChild(0).transform;
        planeMaterial = GetComponent<MeshRenderer>().material;
        holeMaterial = holeTransform.GetComponent<MeshRenderer>().material;

        animationDuration = GetComponent<Animator>()
            .runtimeAnimatorController
            .animationClips[0]
            .length;
    }

    private void Start()
    {
        transform.localScale = new Vector3(
            GetPlaneSize(transform.localScale.x),
            transform.localScale.y,
            GetPlaneSize(transform.localScale.z)
        );

        holeTransform.localScale = new Vector3(
            GetHoleSize(transform.localScale.x),
            holeTransform.localScale.y,
            GetHoleSize(transform.localScale.z)
        );
    }

    public void AddRenderQueue(int platformIndex)
    {
        int queue = MAX_MASK_QUEUE - platformIndex * 2;
        holeMaterial.renderQueue = queue - 1;
        planeMaterial.renderQueue = queue;
    }

    public IEnumerator AnimateScale()
    {
        float startTime = Time.time;
        float targetSize = PLANE_SIZE * scaleFactor;
        float endTime = Time.time + animationDuration;

        Vector3 currentHoleScale = holeTransform.localScale;
        Vector3 currentPlaneScale = transform.localScale;

        Vector3 targetPlaneScale = new Vector3(
            GetPlaneSize(targetSize),
            transform.localScale.y,
            GetPlaneSize(targetSize)
        );

        Vector3 targetHoleScale = new Vector3(
            GetHoleSize(targetSize),
            holeTransform.localScale.y,
            GetHoleSize(targetSize)
        );

        while (Time.time < endTime)
        {
            float time = (Time.time - startTime) / animationDuration;

            transform.localScale = Vector3.Slerp(currentPlaneScale, targetPlaneScale, time);
            holeTransform.localScale = Vector3.Slerp(currentHoleScale, targetHoleScale, time);

            yield return null;
        }

        holeTransform.localScale = targetHoleScale;
        transform.localScale = targetPlaneScale;
    }

    private float GetPlaneSize(float side) =>
        side + borderMargin / HOLE_SIZE;

    private float GetHoleSize(float side) =>
        10.0f * side / GetPlaneSize(side);

    private void Destroy() =>
        Destroy(gameObject, animationDuration / 60.0f);
}
