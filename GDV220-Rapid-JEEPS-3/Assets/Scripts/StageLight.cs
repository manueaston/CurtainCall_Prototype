using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLight : MonoBehaviour
{
    public Light lightComponent;
    public Player player;
    public Transform playerHideCheck;

    private int input;
    private float angle = 0.0f;
    public float rotateSpeed;
    public float angleMin;
    public float angleMax;

    private Color playerColor;
    public Color hiddenColor;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);

        playerColor = player.sr.color;
        hiddenColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.6f);
    }

    private void Update()
    {
        Vector3 newPos = transform.position;

        GetInput();

        float addAngle = input * rotateSpeed * Time.deltaTime;
        if ((angle + addAngle) < angleMin || (angle + addAngle) > angleMax)
        {
            addAngle = 0.0f;
        }

        transform.RotateAround(player.transform.position, Vector3.forward, addAngle);
        angle += addAngle;

        CheckPlayerLit();
    }

    void GetInput()
    {
        input = 0;

        if (Input.GetKey(KeyCode.X))
        {
            input = -1;
        }
        if (Input.GetKey(KeyCode.C))
        {
            input = 1;
        }
    }

    void CheckPlayerLit()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerHideCheck.transform.position - transform.position, out hit, lightComponent.range))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.DrawRay(transform.position, Vector3.Normalize(playerHideCheck.transform.position - transform.position) * hit.distance, Color.yellow);

                player.Hide(false);
                // Debug.Log("Not hidden");
            }
            else
            {
                player.Hide(true);
                // Debug.Log("Hidden");
            }
        }
        else
        {
            player.Hide(true);
            // Debug.Log("Hidden");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1.0f);

        Gizmos.DrawLine(transform.position, playerHideCheck.transform.position);
    }
}
