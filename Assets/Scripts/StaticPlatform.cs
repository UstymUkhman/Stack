using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticPlatform : MonoBehaviour
{
    private bool collided = false;
    public int index = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collided) return;

        bool left = !Convert.ToBoolean(index % 2);

        float offset = left
            ? collision.transform.localPosition.z
            : collision.transform.localPosition.x;

        if (offset == 0)
        {
            Debug.Log("Perfect!");
            return;
        }

        collided = CutPlatform(collision.transform, offset, left);

        Debug.Log("Collided: " + collided);
    }

    private bool CutPlatform(Transform platform, float offset, bool left)
    {
        float halfSide = 0.0f;
        int sign = Math.Sign(offset);

        Vector3 position = platform.position;
        Vector3 platformScale = platform.localScale;
        Vector3 direction = left ? -Vector3.right : Vector3.forward;

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
            return false;
        }

        Destroy(platform.gameObject);
        
        Vector3 size = platform.position + direction * halfSide;
        CreatePlatform(platform, position, size, sign, left, true);

        size = platform.position - direction * halfSide;
        CreatePlatform(platform, position, size, sign, left, false);

        return true;
    }

    private void CreatePlatform(Transform platformTransform, Vector3 position, Vector3 size, int sign, bool left, bool first)
    {
        Vector3 platformScale = platformTransform.localScale;
        Material material = platformTransform.GetComponent<MeshRenderer>().material;

        float platformWidth = Vector3.Distance(position, size);
        GameObject platform = GeneratePlatform(sign, first);

        platform.transform.position = (size + position) / 2;
        platform.transform.parent = transform.parent.transform;
        platform.GetComponent<MeshRenderer>().material = material;

        platform.transform.localScale = left
            ? new Vector3(platformWidth, platformScale.y, platformScale.z)
            : new Vector3(platformScale.x, platformScale.y, platformWidth);
    }

    private GameObject GeneratePlatform(int sign, bool first)
    {
        if ((first && sign == 1) || (!first && sign == -1))
        {
            return Instantiate(gameObject, Vector3.zero, Quaternion.identity, transform);
        }
        else
        {
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.AddComponent<Rigidbody>().mass = 100f;
            return platform;
        }
    }
}
