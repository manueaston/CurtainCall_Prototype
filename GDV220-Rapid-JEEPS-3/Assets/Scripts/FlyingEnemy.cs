using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public CheckIfVisible movementTrigger;
    bool moving = false;

    public float enemySpeed;

    float xDeletePos = -50.0f;

    // Update is called once per frame
    void Update()
    {
        // Move
        if (moving)
        {
            transform.Translate(new Vector3(enemySpeed * Time.deltaTime, 0.0f, 0.0f));
        }
        else // Check if trigger point is visible to camera
        {
            moving = movementTrigger.visible;
        }

        if (transform.position.x < xDeletePos)
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.transform.GetComponent<Player>().Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(movementTrigger.transform.position, 0.2f);
    }
}
