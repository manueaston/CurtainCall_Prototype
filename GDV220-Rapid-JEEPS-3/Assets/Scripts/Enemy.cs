using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemySpeed = 0.005f;
    private bool canMove = true;
    private float originalY;
    private float floatStrength = 0.25f;

    // components
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Light lightComponent;

    public Transform sightRangePoint;
    public float sightRange;

    public float minChangeDirTime;
    public float maxChangeDirTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        lightComponent = transform.GetChild(0).GetComponent<Light>();

        originalY = transform.position.y;

        sightRange = Vector3.Distance(transform.position, sightRangePoint.position);

        StartCoroutine(ChangeViewDirection());
    }

    // Update is called once per frame
    void Update()
    {
        // movement
        if (canMove)
        {
            // movement
            transform.Translate(new Vector3(enemySpeed * Time.deltaTime, 0.0f, 0.0f));
        }

        // checking for player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, sightRange))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.red);
                Debug.Log("Did Hit");

                hit.transform.GetComponent<Player>().Die();

                canMove = false;
                StopCoroutine("ChangeViewDirection");
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.white);
                canMove = false;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * sightRange, Color.white);
        }

        // bobbing
        if (!canMove) return;
        transform.position = new Vector3(
            transform.position.x,
            originalY + ((float)Mathf.Sin(Time.time * 4.0f) * floatStrength),
            transform.position.z
        );
    }

    IEnumerator ChangeViewDirection()
    {
        canMove = true;

        float waitTime = Random.Range(minChangeDirTime, maxChangeDirTime);
        yield return new WaitForSeconds(waitTime);
        canMove = false;
        yield return new WaitForSeconds(3.0f);

        transform.Rotate(0, 180, 0);

        StartCoroutine(ChangeViewDirection());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.transform.GetComponent<Player>().Die(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(sightRangePoint.position, 0.1f);
    }
}
