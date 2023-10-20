using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerLit : MonoBehaviour
{
    Light lightComponent;
    public Player player;
    public Transform playerHideCheck;

    private void Start()
    {
        lightComponent = GetComponent<Light>();
    }

    private void Update()
    {
        PlayerLit();
    }

    void PlayerLit()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerHideCheck.transform.position - transform.position, out hit, lightComponent.range, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.DrawRay(transform.position, Vector3.Normalize(playerHideCheck.transform.position - transform.position) * hit.distance, Color.yellow);

                if (player.hidden)
                {
                    player.Hide(false);
                }
                // Debug.Log("Not hidden");
            }
            else
            {
                if (!player.hidden)
                {
                    player.Hide(true);
                }
                // Debug.Log("Hidden");
            }
        }
        else
        {
            if (!player.hidden)
            {
                player.Hide(true);
            }
            // Debug.Log("Hidden");
        }
    }
}
