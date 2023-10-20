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

    // peeking params (for bossfight)
    bool active = true;
    float peekTime = 4.5f;
    public bool stationary = false;
    bool peeking = false;
    bool lit = false;
    float speed = 0.5f;
    float originalY;

    void Start()
    {
        lightComponent = transform.GetChild(0).GetComponent<Light>();
        spotAngle = lightComponent.spotAngle;

        originalY = transform.position.y;

        StartCoroutine(ChangeViewDirection());
    }

    void Update()
    {
        lightComponent.enabled = active;

        if (!active) return;

        // movement
        if (canMove && !stationary)
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

    public void BossStunned(bool stunned = true)
    {
        if (stunned)
        {
            canMove = false;
            StartCoroutine(Lit());
        }
        else
        {
            canMove = true;
            // transform.Translate(0.0f, enemySpeed, 0.0f);
        }
    }

    public IEnumerator Lit()
    {
        if (!lit)
        {
            Debug.Log("Watcher Lit");

            // deactivate
            active = false;
            lit = true;
            yield return StartCoroutine(Move(-1.9f));

            // reactivate
            yield return new WaitForSeconds(peekTime);
            yield return StartCoroutine(Move(1.9f));
            active = true;
            lit = false;
        }
    }

    IEnumerator Move(float yDisplacement)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + yDisplacement, transform.position.z);
        float startTime = Time.time;

        int counter = 0;
        while (Vector3.Distance(transform.position, endPos) > 0.001 && counter < 1000)
        {
            transform.position = Vector3.Lerp(startPos, endPos, speed * (Time.time - startTime));
            counter++;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ChangeViewDirection()
    {
        float waitTime = 7.5f;
        yield return new WaitForSeconds(waitTime);
        // croak.Play();

        enemySpeed *= -1;

        StartCoroutine(ChangeViewDirection());
    }
}