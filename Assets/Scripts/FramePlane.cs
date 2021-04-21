using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FramePlane : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
