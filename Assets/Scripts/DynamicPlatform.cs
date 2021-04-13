using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicPlatform : MonoBehaviour
{
    private Rigidbody rigidBody;
    private Animator animator;

    // public float offset = 0.0f;
    public int index = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        animator.SetTrigger("Move");
        rigidBody.isKinematic = true;
        rigidBody.detectCollisions = false;
    }

    /* void LateUpdate()
    {
        bool left = Convert.ToBoolean(index % 2);

        gameObject.transform.localPosition = new Vector3(
            left ? offset : gameObject.transform.localPosition.x,
            index,
            left ? gameObject.transform.localPosition.z : offset
        );
    } */

    void LateUpdate()
    {
        bool left = Convert.ToBoolean(index % 2);

        gameObject.transform.localPosition = new Vector3(
            left ? 0.0f : gameObject.transform.localPosition.x,
            index,
            left ? gameObject.transform.localPosition.z : 0.0f
        );
    }

    public void SetDirection(bool left)
    {
        animator.SetBool("Left", left);
    }

    public void Stop()
    {
        animator.enabled = false;
        gameObject.tag = "Untagged";
        animator.ResetTrigger("Move");

        rigidBody.isKinematic = false;
        rigidBody.detectCollisions = true;
    }
}
