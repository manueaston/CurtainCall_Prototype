using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private bool spoken = false;
    public bool stab;
    [SerializeField] private GameObject text;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite wave;
    [SerializeField] private Sprite dead;

    void Start()
    {
        spriteRenderer = spriteRenderer.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idle;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !spoken)
        {
            text.SetActive(true);
            spriteRenderer.sprite = wave;

            other.transform.gameObject.GetComponent<Player>().DialogueStart(this);
            spoken = true;
        }
    }
    
    public void HideText()
    {
        spriteRenderer.sprite = idle;
        text.SetActive(false);

        if (stab)
        {
            spriteRenderer.enabled = false;
            StartCoroutine(DeadSprite());
        }
    }

    IEnumerator DeadSprite()
    {
        yield return new WaitForSeconds(13.05f); // magic number
        transform.position = new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z);
        // transform.localScale = new Vector3(transform.localScale.x - 0.15f, transform.localScale.y - 0.15f, transform.localScale.z - 0.15f);;
        spriteRenderer.sprite = dead;
        spriteRenderer.enabled = true;
    }
}