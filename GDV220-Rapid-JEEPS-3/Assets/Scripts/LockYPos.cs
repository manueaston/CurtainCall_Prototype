using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockYPos : MonoBehaviour
{
    float YPos;

    // Start is called before the first frame update
    void Start()
    {
        YPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, YPos, transform.position.z);
    }
}
