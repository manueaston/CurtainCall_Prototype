using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Transform target;
    public bool autoFollow = true;

    void FixedUpdate()
    {
        if (autoFollow)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));
        }
    }
}
