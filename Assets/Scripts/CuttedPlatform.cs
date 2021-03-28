using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CuttedPlatform : MonoBehaviour
{
    void FixedUpdate()
    {
        Debug.Log(gameObject.transform.position);
        Debug.Log(gameObject.transform.localPosition);
    }
}
