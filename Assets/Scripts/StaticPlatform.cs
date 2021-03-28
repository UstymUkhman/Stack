using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticPlatform : MonoBehaviour
{
    public GameObject cuttedPrefab;
    public int index = 0;
    public int sign = 0;

    private void OnCollisionEnter(Collision collision)
    {
        bool left = !Convert.ToBoolean(index % 2);

        float offset = left
            ? collision.transform.localPosition.z
            : collision.transform.localPosition.x;

        sign = Math.Sign(offset);

        if (offset == 0)
        {
            Debug.Log("Perfect!");
            return;
        }

        bool collided = CutPlatform(collision.transform, left);

        if (!collided)
        {
            Debug.Log("Game Over!");
        }
    }

    private bool CutPlatform(Transform platform, bool left)
    {
        float halfSide = 0.0f;

        Vector3 platformScale = platform.localScale;
        Vector3 direction = left ? -Vector3.right : Vector3.forward;
        Vector3 position = new Vector3(platform.position.x, platform.position.y, platform.position.z);

        if (left)
        {
            position.x = transform.position.x + transform.localScale.x / (sign * 2);
            halfSide = platformScale.x / 2;
        }
        else
        {
            position.z = transform.position.z + transform.localScale.z / (sign * -2);
            halfSide = platformScale.z / 2;
        }

        float distance = Vector3.Distance(platform.position, position);

        if (distance >= halfSide)
        {
            if (left)
            {
                position.x = transform.position.x + transform.localScale.x / (sign * -2);
            }
            else
            {
                position.z = transform.position.z + transform.localScale.z / (sign * 2);
            }

            direction = -direction;
        }

        Destroy(platform.gameObject);

        Vector3 size = platform.position + direction * halfSide;
        CreatePlatform(platform, position, size, left);

        size = platform.position - direction * halfSide;
        CreatePlatform(platform, position, size, left);

        return true;
    }

    private void CreatePlatform(Transform platformTransform, Vector3 position, Vector3 size, bool left)
    {
        GameObject platform = GeneratePlatform();
        Vector3 platformScale = platformTransform.localScale;

        float platformWidth = Vector3.Distance(position, size);
        Material material = platformTransform.GetComponent<MeshRenderer>().material;

        platform.name = "Platform" + (index + 1);

        platform.transform.position = (size + position) / 2;
        platform.transform.parent = transform.parent.transform;
        platform.GetComponent<MeshRenderer>().material = material;

        platform.transform.localScale = left
            ? new Vector3(platformWidth, platformScale.y, platformScale.z)
            : new Vector3(platformScale.x, platformScale.y, platformWidth);
    }

    private GameObject GeneratePlatform()
    {
        // return Instantiate(gameObject, Vector3.zero, Quaternion.identity, transform);
        return Instantiate(cuttedPrefab, Vector3.zero, Quaternion.identity, transform);
    }
}
