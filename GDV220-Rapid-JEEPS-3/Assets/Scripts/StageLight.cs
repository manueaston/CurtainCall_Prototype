using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLight : MonoBehaviour
{
    public Light lightComponent;
    public Player player;
    public GameObject boss;

    //private int input;
    //private float angle = 0.0f;
    //public float rotateSpeed;
    //public float angleMin;
    //public float angleMax;

    // METHOD 3 Variables
    //private Vector2 input2D;
    //public float moveSpeed;
    //public float returnSpeed;
    float playerZDifference;

    public float maxXDist;
    public float minZPos;
    public float maxZPos;

    //const float timeBeforeReturn = 2.0f;
    //float countdownTimer = timeBeforeReturn;

    // METHOD 4 Variables
    private Vector3 moveVector;
    private Vector3 currentLookPos;
    private Vector3 targetLookPos;
    private float targetYPos;
    public float moveSpeed;
    
    private void Start()
    {
        // Detach from player
        transform.parent = null;

        lightComponent = GetComponent<Light>();
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);

        playerZDifference = player.transform.position.z - transform.position.z;
        currentLookPos = new Vector3(player.transform.position.x, targetYPos, player.transform.position.z - playerZDifference);

        GameObject floor = GameObject.Find("Floor");
        targetYPos = floor.transform.position.y + floor.GetComponent<BoxCollider>().bounds.size.y / 2;
    }

    private void Update()
    {
        GetInput();

        transform.Translate(moveVector, Space.World);


        //transform.position = new Vector3(targetLookPos.x, transform.position.y, targetLookPos.z + playerZDifference);

        Vector3 playerTargetPos = new Vector3(player.transform.position.x, targetYPos, player.transform.position.z - playerZDifference);
        CheckBounds(playerTargetPos);

        //Debug.Log(transform.position);

        //if (input2D == Vector2.zero)
        //{
        //    if (Vector3.Distance(transform.position, playerTargetPos) > 0.01f)
        //    {
        //        if (countdownTimer <= 0.0f)
        //        {
        //            // Move back to player position
        //            moveVector = playerTargetPos - transform.position;
        //            moveVector.y = 0.0f;
        //            moveVector.Normalize();
        //            moveVector *= returnSpeed * Time.deltaTime;

        //            transform.Translate(moveVector, Space.World);
        //        }
        //        else
        //        {
        //            countdownTimer -= Time.deltaTime;
        //        }
        //    }
        //}
        //else
        //{
        //    countdownTimer = timeBeforeReturn;
        //}

        CheckEnemyLit();
    }

    void GetInput()
    {
        moveVector = new Vector3(0.0f, 0.0f, 0.0f);

        // Mouse Input //
        Ray ray = new Ray(transform.position, transform.forward);
        Plane xz = new Plane(Vector3.up, new Vector3(0, targetYPos, 0));
        float distance;

        // Current Look Pos
        xz.Raycast(ray, out distance);
        currentLookPos = ray.GetPoint(distance);

        // Target Look Pos
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        xz.Raycast(ray, out distance);

        if (distance > 0.0f)
        {
            targetLookPos = ray.GetPoint(distance);

            moveVector = targetLookPos - currentLookPos;
            moveVector.Normalize();
            moveVector *= moveSpeed * Time.deltaTime;
        }
    }

    void CheckEnemyLit()
    {
        var radians = lightComponent.spotAngle * Mathf.Deg2Rad;
        var x = Mathf.Cos(radians);
        var y = Mathf.Sin(radians);
        var pos = new Vector3(x, y, 0);

        RaycastHit hit; // TODO: transform.forward may not be the best option?
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(lightComponent.spotAngle, -transform.up) * transform.forward * lightComponent.range, Color.green);
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(lightComponent.spotAngle, -transform.up) * transform.forward, out hit, lightComponent.range))
        {
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(lightComponent.spotAngle, -transform.up) * transform.forward * lightComponent.range, Color.red);

            if (hit.transform.tag == "Boss")
            {
                boss.GetComponent<Bossfight>().Stun();
            }
            else if (hit.transform.tag == "Enemy")
            {
                // disable watcher enemy for 5s
                WatcherEnemy bullFrog = hit.transform.GetComponent<WatcherEnemy>();
                if (bullFrog && !bullFrog.IsLit())
                {
                    StartCoroutine(bullFrog.Lit());
                }
                else
                {
                    Enemy hammerHead = hit.transform.GetComponent<Enemy>();
                    if (hammerHead)
                    {
                        StartCoroutine(hammerHead.Wait());
                    }
                }
            }
        }
    }
        
    void CheckBounds(Vector3 _playerPos)
    {
        Vector3 newPos = transform.position;

        if (transform.position.x < _playerPos.x - maxXDist)
        {
            newPos.x = _playerPos.x - maxXDist;
        }
        else if (transform.position.x > _playerPos.x + maxXDist)
        {
            newPos.x = _playerPos.x + maxXDist;
        }

        if (transform.position.z < minZPos - playerZDifference)
        {
            newPos.z = minZPos - playerZDifference;
        }
        else if (transform.position.z > maxZPos - playerZDifference)
        {
            newPos.z = maxZPos - playerZDifference;
        }

        transform.position = newPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetLookPos, 0.5f);
        Gizmos.DrawLine(transform.position, targetLookPos);
        // Gizmos.DrawLine(transform.position, boss.transform.position);
    }
}