using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherEnemy : MonoBehaviour
{
    public Light lightComponent;
    private float maxIntensity;

    public float minWatchTime;
    public float maxWatchTime;

    public float peekTime;
    bool peeking = false;

    public float minWaitTime;
    public float maxWaitTime;

    bool active;

    private float spotAngle;

    public float speed;

    public AudioSource croak;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = transform.GetChild(0).GetComponent<Light>();
        maxIntensity = lightComponent.intensity;
        active = false;

        spotAngle = lightComponent.spotAngle;

        StartCoroutine(Show());
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }


        Vector3 lightDirection = lightComponent.transform.TransformDirection(Vector3.forward);
   
        RaycastHit hit;
        for (float angle = (spotAngle / 2); angle > -(spotAngle / 2); angle -= (spotAngle / 10))
        {
            if (Physics.Raycast(transform.position, Quaternion.AngleAxis(angle, Vector3.up) * lightDirection, out hit, lightComponent.range))
            {
                if (hit.transform.tag == "Player")
                {
                    Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, Vector3.up) * lightDirection * hit.distance, Color.red);

                    Debug.Log("Player Spotted");
                    hit.transform.GetComponent<Player>().Die();
                    
                    break;
                }
            }
        }
    }

    IEnumerator Show()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        // Peek
        yield return StartCoroutine(Move(0.4f));
        yield return StartCoroutine(Move(-0.5f));
        yield return StartCoroutine(Move(0.5f));
        yield return StartCoroutine(Move(-0.2f));
        yield return StartCoroutine(Move(0.6f));
        yield return StartCoroutine(Move(-0.8f));
        yield return new WaitForSeconds(peekTime);

        // Activate
        yield return StartCoroutine(Move(1.9f));
        active = true;
        yield return new WaitForSeconds(Random.Range(minWatchTime, maxWatchTime));

        // Deactivate
        yield return StartCoroutine(Move(-1.9f));
        active = false;

        StartCoroutine(Show());
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

    IEnumerator Flicker()
    {
        Debug.Log("Flicker");
        lightComponent.intensity = 0;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
        lightComponent.intensity = maxIntensity;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
        croak.Play();

        if (peeking)
        {
            StartCoroutine(Flicker());
        }
    }
}
