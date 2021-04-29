using UnityEngine;

public class CuttedPlatform : MonoBehaviour
{
    private const float BOTTOM_LIMIT = -4.0f;
    private float MASS_MULTIPLIER;
    private Rigidbody body;

    private void Awake()
    {
        body = gameObject.GetComponent<Rigidbody>();
        MASS_MULTIPLIER = body.mass;
    }

    private void Start()
    {
        Vector3 scale = gameObject.transform.localScale;
        body.mass = scale.x * scale.y * scale.z * MASS_MULTIPLIER;
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.localPosition.y < BOTTOM_LIMIT)
        {
            Destroy(gameObject);
        }
    }
}
