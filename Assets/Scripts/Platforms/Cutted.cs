using UnityEngine;

public class Cutted : MonoBehaviour
{
    private const float BOTTOM_LIMIT = -4.0f;
    private Rigidbody body;

    private void Awake()
    {
        body = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Vector3 scale = gameObject.transform.localScale;
        body.mass *= scale.x * scale.y * scale.z;
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.localPosition.y < BOTTOM_LIMIT)
        {
            Destroy(gameObject);
        }
    }
}
