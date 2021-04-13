using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticPlatform : MonoBehaviour
{
    public GameObject cuttedPrefab;
    private bool collided = false;

    private int sign = 0;
    public int index = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collided || collision.gameObject.isStatic) return;

        float offset = !Convert.ToBoolean(index % 2)
            ? collision.transform.localPosition.z
            : collision.transform.localPosition.x;

        sign = Math.Sign(offset);

        // Debug.Log("Offset: " + offset + " | Sign: " + sign);

        if (offset == 0)
        {
            Debug.Log("Perfect!");
            return;
        }

        collided = CutPlatform(collision.transform);

        if (collided)
        {
            Debug.Log("Collision!");
        }
        else
        {
            Debug.Log("Game Over!");
        }
    }

    private bool CutPlatform(Transform platform)
    {
        Vector3 position = platform.position;
        // Vector3 position = new Vector3(platform.position.x, platform.position.y, platform.position.z);

        if (Convert.ToBoolean(index % 2))
        {
            position.z = transform.position.z + transform.localScale.z / (sign * -2);
        }
        else
        {
            position.x = transform.position.x + transform.localScale.x / (sign * 2);
        }

        /*float distance = Vector3.Distance(platform.position, position);

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
        }*/

        Destroy(platform.gameObject);

        // Left: true:
        //   Sign == +1: static <-- dynamic | offset > 0
        //   Sign == -1: dynamic <-- static | offset < 0

        // Left: false:
        //   Sign == -1: dynamic --> static | offset < 0
        //   Sign == +1: static --> dynamic | offset > 0

        CreatePlatform(platform, position, 1, sign < 0);
        CreatePlatform(platform, position, -1, sign > 0);

        return true;
    }

    private void CreatePlatform(Transform platform, Vector3 position, int side, bool dynamic)
    {
        bool left = !Convert.ToBoolean(index % 2);
        Vector3 platformScale = platform.localScale;

        Vector3 direction = left ? -Vector3.right : Vector3.forward;
        float halfSide = left ? platformScale.x / 2 : platformScale.z / 2;

        GameObject prefab = Instantiate(dynamic ? cuttedPrefab : gameObject, Vector3.zero, Quaternion.identity, transform);

        Vector3 size = platform.position + direction * halfSide * side;
        SetPlatformTransform(prefab, platform, position, size);
    }

    private void SetPlatformTransform(GameObject platform, Transform platformTransform, Vector3 position, Vector3 size)
    {
        Vector3 platformScale = platformTransform.localScale;

        float platformWidth = Vector3.Distance(position, size);
        Material material = platformTransform.GetComponent<MeshRenderer>().material;

        platform.name = "Platform" + (index + 1);

        platform.transform.position = (size + position) / 2;
        platform.transform.parent = transform.parent.transform;
        platform.GetComponent<MeshRenderer>().material = material;

        platform.transform.localScale = !Convert.ToBoolean(index % 2)
            ? new Vector3(platformWidth, platformScale.y, platformScale.z)
            : new Vector3(platformScale.x, platformScale.y, platformWidth);
    }
}
