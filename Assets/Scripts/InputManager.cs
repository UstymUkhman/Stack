using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StackManager.instance.Stop();
            StackManager.instance.Spawn();
        }
    }
}
