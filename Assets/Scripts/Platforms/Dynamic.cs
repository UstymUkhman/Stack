using UnityEngine;

public class Dynamic : MonoBehaviour
{
    [HideInInspector] public float offset { set; private get; }
    [HideInInspector] public float y { set; private get; }

    private Animator animator;
    private bool moveLeft;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        float x = gameObject.transform.localPosition.x;
        float z = gameObject.transform.localPosition.z;

        x = moveLeft ? x : x + offset;
        z = moveLeft ? z + offset : z;

        gameObject.transform.localPosition = new Vector3(
            x, y, z
        );
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

    public void Stop()
    {
        animator.ResetTrigger("Move");
        animator.enabled = false;
    }
}
