using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatfomManager : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigidBody;

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

    public void SetDirection(bool left)
    {
        animator.SetBool("Left", left);
    }

    /*void LateUpdate()
    {
        Vector3 pos = gameObject.transform.position;
        pos.y = position.y;
        gameObject.transform.position = pos;
    }*/


    public void Stop()
    {
        animator.enabled = false;
        gameObject.tag = "Untagged";
        animator.ResetTrigger("Move");

        rigidBody.isKinematic = false;
        rigidBody.detectCollisions = true;
    }
}
