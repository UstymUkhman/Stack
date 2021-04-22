using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformPlane : MonoBehaviour
{
    // [Header("Margin from plane border (range 0.0 - 1.0):")]
    // [SerializeField] private float holeMargin = 0.2f;

    private readonly int MASK_QUEUE = 3002;
    private float animationDuration;

    void Start()
    {
        Transform holeTransform = transform.GetChild(0).transform;

        bool deep = transform.localScale.x < transform.localScale.z;
        float minSize = deep ? transform.localScale.x : transform.localScale.z;

        float size = 8.0f * minSize / 0.12f;

        Vector3 scale = new Vector3(
            deep ? size : 8.0f,
            holeTransform.localScale.y,
            deep ? 8.0f : size
        );

        holeTransform.localScale = scale;
        GetComponent<MeshRenderer>().material.renderQueue = MASK_QUEUE;

        animationDuration = GetComponent<Animator>()
            .runtimeAnimatorController
            .animationClips[0]
            .length;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
