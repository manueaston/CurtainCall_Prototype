using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossfightCutsceneTrigger : MonoBehaviour
{
    [SerializeField] private Bossfight bossfight;
    private bool spoken = false;
    [SerializeField] private GameObject text;

    void Start()
    {
        bossfight.bossLight.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !spoken)
        {
            text.SetActive(true);
            bossfight.bossLight.SetActive(true);

            other.transform.gameObject.GetComponent<Player>().DialogueStart(this);
            spoken = true;
        }
    }
    
    public void HideText()
    {
        text.SetActive(false);
    }
}
