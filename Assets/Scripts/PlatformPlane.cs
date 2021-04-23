using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformPlane : MonoBehaviour
{
    [Header("Margin from plane border (range 0.0 - 1.0):")]
    [SerializeField] private float borderMargin = 0.1f;

    private readonly float MAX_SIZE = 10.0f;
    private readonly int MASK_QUEUE = 3002;
    // private float animationDuration;

    void Start()
    {
        float margin = borderMargin / MAX_SIZE;

        float x = transform.localScale.x + margin;
        float z = transform.localScale.z + margin;

        float width = 10.0f * transform.localScale.x / x;
        float depth = 10.0f * transform.localScale.z / z;

        Transform holeTransform = transform.GetChild(0).transform;
        transform.localScale = new Vector3(x, transform.localScale.y, z);

        holeTransform.localScale = new Vector3(width, holeTransform.localScale.y, depth);
        GetComponent<MeshRenderer>().material.renderQueue = MASK_QUEUE;

        /*animationDuration = GetComponent<Animator>()
            .runtimeAnimatorController
            .animationClips[0]
            .length;*/
    }

    private void Destroy()
    {
        // Destroy(gameObject);
    }
}
