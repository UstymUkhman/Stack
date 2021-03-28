using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformCutter : MonoBehaviour
{
    public bool CutHorizontally(Transform platform, float z)
    {
        Vector3 platformScale = platform.localScale;
        Material material = platform.GetComponent<MeshRenderer>().material;

        Vector3 position = new Vector3(platform.position.x, platform.position.y, z);
        float distance = Vector3.Distance(platform.position, position);

        if (distance >= platformScale.z / 2)
        {
            return false;
        }

        Destroy(platform.gameObject);

        Vector3 top = platform.position + Vector3.forward * platformScale.z / 2;
        GameObject topPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float topPlatformDepth = Vector3.Distance(position, top);

        topPlatform.transform.position = (top + position) / 2;
        topPlatform.transform.parent = transform;

        topPlatform.transform.localScale = new Vector3(platformScale.x, platformScale.y, topPlatformDepth);
        topPlatform.GetComponent<MeshRenderer>().material = material;
        topPlatform.AddComponent<Rigidbody>().mass = 100f;

        Vector3 bottom = platform.position - Vector3.forward * platformScale.z / 2;
        GameObject bottomPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float bottomPlatformDepth = Vector3.Distance(position, bottom);

        bottomPlatform.transform.position = (bottom + position) / 2;
        bottomPlatform.transform.parent = transform;

        bottomPlatform.transform.localScale = new Vector3(platformScale.x, platformScale.y, bottomPlatformDepth);
        bottomPlatform.GetComponent<MeshRenderer>().material = material;
        bottomPlatform.AddComponent<Rigidbody>().mass = 100f;

        return true;
    }

    public bool CutVertically(Transform platform, float x)
    {
        Vector3 victimScale = platform.localScale;
        Material material = platform.GetComponent<MeshRenderer>().material;
        Vector3 position = new Vector3(x, platform.position.y, platform.position.z);

        float distance = Vector3.Distance(platform.position, position);

        if (distance >= victimScale.x / 2)
        {
            return false;
        }

        Destroy(platform.gameObject);

        Vector3 left = platform.position - Vector3.right * victimScale.x / 2;
        GameObject leftPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float leftPlatformWidth = Vector3.Distance(position, left);

        leftPlatform.transform.position = (left + position) / 2;
        leftPlatform.transform.parent = transform;

        leftPlatform.transform.localScale = new Vector3(leftPlatformWidth, victimScale.y, victimScale.z);
        leftPlatform.GetComponent<MeshRenderer>().material = material;
        leftPlatform.AddComponent<Rigidbody>().mass = 100f;

        Vector3 right = platform.position + Vector3.right * victimScale.x / 2;
        GameObject rightPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float rightPlatformWidth = Vector3.Distance(position, right);

        rightPlatform.transform.position = (right + position) / 2;
        rightPlatform.transform.parent = transform;
                
        rightPlatform.transform.localScale = new Vector3(rightPlatformWidth, victimScale.y, victimScale.z);
        rightPlatform.GetComponent<MeshRenderer>().material = material;
        rightPlatform.AddComponent<Rigidbody>().mass = 100f;

        return true;
    }
}
