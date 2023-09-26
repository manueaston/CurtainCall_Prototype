using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingWatcherEnemy : MonoBehaviour
{
    public float enemySpeed = 0.005f;
    private bool canMove = true;

    public Light lightComponent;

    private float spotAngle;
    
    public AudioSource croak;

    void Start()
    {
        lightComponent = transform.GetChild(0).GetComponent<Light>();
        spotAngle = lightComponent.spotAngle;

        StartCoroutine(ChangeViewDirection());
    }

    void Update()
    {
        // movement
        if (canMove)
        {
            // movement
            transform.Translate(new Vector3(enemySpeed * Time.deltaTime, 0.0f, 0.0f));
        }

        Vector3 lightDirection = lightComponent.transform.TransformDirection(Vector3.forward);
   
        RaycastHit hit;
        for (float angle = (spotAngle / 2) - 10.0f ; angle > -(spotAngle / 2) + 10.0f; angle -= (spotAngle / 10)) // adding & subtracting for leniency
        {
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(angle, Vector3.up) * lightDirection, out hit, lightComponent.range))
            {
                if (hit.transform.tag == "Player")
                {
                    Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, Vector3.up) * lightDirection * hit.distance, Color.red);

                    Player player = hit.transform.GetComponent<Player>();
                    if (player && !player.hidden)
                    {
                        Debug.Log("Player Spotted");
                        player.Die();
                    }
                    
                    break;
                }
            }
        }
    }

    IEnumerator ChangeViewDirection()
    {
        float waitTime = 7.5f;
        yield return new WaitForSeconds(waitTime);
        croak.Play();

        enemySpeed *= -1;

        StartCoroutine(ChangeViewDirection());
    }
}