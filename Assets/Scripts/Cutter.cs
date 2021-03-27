using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 pos = transform.position;

        // pos.x = pos.x + transform.localScale.x / 2;

        pos.x = pos.x - transform.localScale.x / 2;

        CubeCut.Cut(collision.transform, pos.x);
    }
}
