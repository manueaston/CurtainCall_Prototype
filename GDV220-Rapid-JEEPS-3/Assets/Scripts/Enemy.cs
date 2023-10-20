using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // components
    Rigidbody rb;
    SpriteRenderer sr;
    Light lightComponent;
    Animator anim;
    public AudioSource audioSource;

    // movement variables
    public float enemySpeed;
    public float enemyLungeSpeedMultiplier;
    public float waitTime;
    public float range;

    private bool canMove = true;
    private Vector2 originalPos;
    private float floatStrength = 0.25f;
    private bool lunging = false;
    private int direction;

    // sight variables
    public Transform sightRangePoint;
    private float sightRange;

    // bossfight params
    bool active = true;
    [SerializeField] private float visibleEnemyHeight;
    [SerializeField] private float invisibleEnemyHeight;
    public bool boss = false;
    private bool starting = true;
    public bool bossStunned = false;

    public void IsBossfight()
    {
        boss = true;
        originalPos = new Vector2(transform.position.x, -0.8f);
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        lightComponent = transform.GetChild(0).GetComponent<Light>();
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        originalPos = transform.position;
        if (transform.rotation.y == 0.0f)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        sightRange = Vector3.Distance(transform.position, sightRangePoint.position);
    }

    void Update()
    {
        lightComponent.enabled = active; // && canMove

        // drop down from top of screen in bossfight
        if (boss && starting)
        {
            originalPos = new Vector2(transform.position.x, -0.8f);
            if (transform.position.y > originalPos.y)
            {
                transform.Translate(0.0f, -enemySpeed * 5.0f * Time.deltaTime, 0.0f);
            }
            else
            {
                transform.GetChild(0).GetComponent<LockYPos>().YPos = transform.position.y - 0.033f;
                starting = false;
            }
            return;
        }

        if (boss)
        {
            if (bossStunned && transform.position.y > invisibleEnemyHeight)
            {
                transform.Translate(0.0f, -enemySpeed * 3.0f * Time.deltaTime, 0.0f);
            }
            else if (!bossStunned && transform.position.y < visibleEnemyHeight)
            {
                transform.Translate(0.0f, enemySpeed * 3.0f * Time.deltaTime, 0.0f);
            }
        }

        if (!active) return;

        if (canMove)
        {
            // horizontal movement
            transform.Translate(new Vector3(enemySpeed * Time.deltaTime, 0.0f, 0.0f));

            // bobbing
            transform.position = new Vector3(
            transform.position.x,
            originalPos.y + ((float)Mathf.Sin(Time.time * 4.0f) * floatStrength),
            transform.position.z);

            if (!lunging && (transform.position.x < originalPos.x - range || transform.position.x > originalPos.x + range))
            {
                if (transform.position.x < originalPos.x - range)
                {
                    transform.position = new Vector3(originalPos.x - range, transform.position.y, transform.position.z);
                }
                else if (transform.position.x > originalPos.x + range)
                {
                    transform.position = new Vector3(originalPos.x + range, transform.position.y, transform.position.z);
                }

                StartCoroutine(Rotate());
            }
        }

        // checking for player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, sightRange))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.red);

                StartCoroutine(Lunge());
            }
            else if (canMove) // Doesn't trigger if enemy is waiting
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.white);
                StartCoroutine(Rotate());
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * sightRange, Color.white);
        }
    }

    public void BossStunned(bool stunned = true)
    {
        bossStunned = stunned;
        if (stunned)
        {
            canMove = false;
            active = false;
        }
        else
        {
            canMove = true;
            active = true;
        }
    }

    IEnumerator Lunge()
    {
        if (lunging) yield break;

        StopCoroutine(Rotate());
        // anim.SetTrigger("Lunge");
        lunging = true;
        canMove = true;

        enemySpeed *= enemyLungeSpeedMultiplier;
        yield return new WaitForSeconds(2.0f);
        enemySpeed /= enemyLungeSpeedMultiplier;

        StartCoroutine(Rotate());
        lunging = false;
        // anim.ResetTrigger("Lunge");
    }

    public IEnumerator Rotate()
    {
        yield return Wait();

        // Only rotate if it would be facing the right direction
        if ((direction == -1 && transform.position.x < originalPos.x)
            || (direction == 1 && transform.position.x > originalPos.x))
        {
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            direction *= -1;
        }

    }
    public IEnumerator Wait()
    {
        if (!lunging)
        {
            canMove = false;
            yield return new WaitForSeconds(waitTime);
            canMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && !bossStunned)
        {
            other.transform.GetComponent<Player>().Die(true);
            canMove = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(sightRangePoint.position, 0.1f);
    }
}