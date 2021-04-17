using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticPlatform : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Animate()
    {
        animator.enabled = true;
        animator.SetTrigger("Move");
    }
}
