using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public float minXPos;
    public float maxXPos;

    void FixedUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);

        // limit position
        if (transform.position.x < minXPos)
        {
            transform.position = new Vector3(minXPos, transform.position.y, transform.position.z);
            return;
        }
        if (transform.position.x > maxXPos)
        {
            transform.position = new Vector3(maxXPos, transform.position.y, transform.position.z);
            return;
        }

        transform.LookAt(target);
    }
}