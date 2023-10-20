using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockYPos : MonoBehaviour
{
    public float YPos;

    void Start()
    {
        YPos = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, YPos, transform.position.z);
    }
}
