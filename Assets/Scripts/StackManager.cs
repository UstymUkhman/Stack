using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    [SerializeField]
    private GameObject platformPrefab;
    public static StackManager instance;

    private Quaternion rotation = Quaternion.Euler(0, 0, 0);

    private PlatfomManager platfomManager;
    private GameObject lastPlatform;
    private int platforms = 0;

    private bool collided = false;

    void Awake()
    {
        if (instance is null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Spawn();
    }

    public void Stop()
    {
        platfomManager.Stop();
    }

    public void Spawn()
    {
        bool moveLeft = !Convert.ToBoolean(platforms % 2);

        Vector3 position = moveLeft
            ? new Vector3(0, 1, 1.5f)
            : new Vector3(-1.5f, 1, 0);

        lastPlatform = Instantiate(platformPrefab, position, rotation, transform);
        platfomManager = lastPlatform.GetComponent<PlatfomManager>();
        platfomManager.SetDirection(moveLeft);

        /*position.x = 0f;
        position.y = ++platforms * 0.1f;
        position.z = 0f;

        transform.position = position;*/
        platforms++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collided) return;

        Vector3 pos = transform.position;
        bool moveLeft = Convert.ToBoolean(platforms % 2);

        float offset = moveLeft
            ? collision.transform.localPosition.z
            : collision.transform.localPosition.x;

        if (offset == 0)
        {
            Debug.Log("Perfect!");
        }
        else if (offset > 0)
        {
            pos.x = pos.x + transform.localScale.x / 2;
        }
        else
        {
            pos.x = pos.x - transform.localScale.x / 2;
        }

        collided = CutPlatform(collision.transform, pos.x);

        Debug.Log("collided: " + collided);
    }

    private bool CutPlatform(Transform victim, float _posX)
    {
        Vector3 victimScale = victim.localScale;
        Material mat = victim.GetComponent<MeshRenderer>().material;
        Vector3 pos = new Vector3(_posX, victim.position.y, victim.position.z);

        float distance = Vector3.Distance(victim.position, pos);
        if (distance >= victimScale.x / 2) return false;

        Vector3 leftPoint = victim.position - Vector3.right * victimScale.x / 2;
        Vector3 rightPoint = victim.position + Vector3.right * victimScale.x / 2;

        Destroy(victim.gameObject);

        GameObject rightSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightSideObj.transform.position = (rightPoint + pos) / 2;
        rightSideObj.transform.parent = transform;

        float rightWidth = Vector3.Distance(pos, rightPoint);
        rightSideObj.transform.localScale = new Vector3(rightWidth, victimScale.y, victimScale.z);

        rightSideObj.GetComponent<MeshRenderer>().material = mat;
        rightSideObj.AddComponent<Rigidbody>().mass = 100f;

        GameObject leftSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftSideObj.transform.position = (leftPoint + pos) / 2;
        leftSideObj.transform.parent = transform;

        float leftWidth = Vector3.Distance(pos, leftPoint);
        leftSideObj.transform.localScale = new Vector3(leftWidth, victimScale.y, victimScale.z);

        leftSideObj.GetComponent<MeshRenderer>().material = mat;
        leftSideObj.AddComponent<Rigidbody>().mass = 100f;

        return true;
    }
}
