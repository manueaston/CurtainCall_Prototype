using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    private Vector3 velocityMove = Vector3.zero;
    private Vector3 velocityTilt = Vector3.zero;

    public float minXPos;
    public float maxXPos;

    private void Start()
    {
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        //transform.LookAt(new Vector3(target.position.x, target.position.y, target.position.z));
    }

    void FixedUpdate()
    {
        Vector3 desiredPos = new Vector3(target.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocityMove, smoothSpeed);

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

        //transform.LookAt(new Vector3(target.position.x, target.position.y, target.position.z));

        //Vector3 desiredLook = new Vector3(target.position.x, target.position.y, target.position.z);
        //transform.LookAt(Vector3.SmoothDamp(transform.position + transform.forward, desiredLook, ref velocityTilt, smoothSpeed));
    }
}