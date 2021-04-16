using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicPlatform : MonoBehaviour
{
    private Animator animator;

    private float offset = 0;
    private bool moveLeft;
    private float y = 0;

    public float Offset
    {
        set { offset = value; }
        get { return offset; }
    }

    public float Y
    {
        set { y = value; }
        get { return y; }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetDirection(bool left)
    {
        animator.SetBool("Left", left);
        moveLeft = left;
    }

    public void Move()
    {
        animator.SetTrigger("Move");
    }

    void LateUpdate()
    {
        float X = gameObject.transform.localPosition.x;
        float Z = gameObject.transform.localPosition.z;

        X = moveLeft ? X : X + Offset;
        Z = moveLeft ? Z + Offset : Z;

        gameObject.transform.localPosition = new Vector3(
            X, Y, Z
        );
    }

    public void Stop()
    {
        animator.enabled = false;
        animator.ResetTrigger("Move");
    }
}
