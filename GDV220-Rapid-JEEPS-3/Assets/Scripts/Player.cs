using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // movement params
    public float playerSpeed;
    private bool dead = false;

    // components
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Animator anim;
    public CapsuleCollider coll;
    public LevelTransition deathTransition;

    // layer params
    public float layerGap = 1.8f;
    public int layer = 0;
    public Transform layerCheckStart;
    public float layerSpeed;

    // crouching params
    bool crouched = false;
    float standingHeight;
    float crouchingHeight;

    // player is in shadow, cannot be seen by enemies
    public bool hidden = false;
    public Transform playerHideCheck;

    public RuntimeAnimatorController animControllerVisible;
    public RuntimeAnimatorController animControllerHidden;

    // audio
    public AudioSource walk;
    public AudioSource crouch;
    public AudioSource layerUp;
    public AudioSource layerDown;
    public AudioSource deathSound;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        coll = gameObject.GetComponent<CapsuleCollider>();
        //sr = gameObject.GetComponent<SpriteRenderer>(); // Sprite renderer and animator on child object of prefab so that sprite is lit from front
        // anim = gameObject.GetComponent<Animator>();


        standingHeight = coll.height;
        crouchingHeight = standingHeight / 2.0f;
    }

    void Update()
    {
        if (dead) return;

        float x = Input.GetAxis("Horizontal");

        if (!crouched)
        {
            // if (Mathf.Abs(x) > 0 && !walk.isPlaying)
            // {
            //     walk.Play();
            // }
            // else if (Mathf.Abs(x) == 0)
            // {
            //     walk.Stop();
            // }

            Vector3 moveDir = new Vector3(x, 0, 0);
            anim.SetFloat("Speed", Mathf.Abs(x));
            rb.velocity = moveDir * playerSpeed;

            // change layer
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && layer < 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(layerCheckStart.position, transform.TransformDirection(Vector3.forward), out hit, 2))
                {
                    // Something is in way

                    if (hit.transform.tag == "Enemy")
                    {
                        layerUp.Play();
                        layer++;
                        //transform.position = new Vector3(transform.position.x, transform.position.y, layerGap * layer);
                        anim.SetTrigger("MoveLayer");
                        StartCoroutine(MoveLayer());
                    }
                }
                else
                {
                    layerUp.Play();
                    layer++;
                    //transform.position = new Vector3(transform.position.x, transform.position.y, layerGap * layer);
                    anim.SetTrigger("MoveLayer");
                    StartCoroutine(MoveLayer());
                }
            }
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && layer > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(layerCheckStart.position, transform.TransformDirection(-Vector3.forward), out hit, 2))
                {
                    // Something is in way

                    if (hit.transform.tag == "Enemy")
                    {
                        layerDown.Play();
                        layer--;
                        //transform.position = new Vector3(transform.position.x, transform.position.y, layerGap * layer);
                        anim.SetTrigger("MoveLayer");
                        StartCoroutine(MoveLayer());
                    }
                }
                else
                {
                    layerDown.Play();
                    layer--;
                    //transform.position = new Vector3(transform.position.x, transform.position.y, layerGap * layer);
                    anim.SetTrigger("MoveLayer");
                    StartCoroutine(MoveLayer());
                }
            }
        }
    

        // Flip sprite
        if (x != 0 && x < 0)
        {
            sr.flipX = true;
        }
        else if (x != 0 && x > 0)
        {
            sr.flipX = false;
        }

        // Crouch
        if ((Input.GetKeyDown(KeyCode.LeftControl)))
        {
            crouch.Play();

            crouched = !crouched;

            if (crouched)
            {
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                coll.height = crouchingHeight;
                coll.center = new Vector3(coll.center.x, coll.center.y - crouchingHeight / 2, coll.center.z);
                playerHideCheck.localPosition = new Vector3(0.0f, - crouchingHeight / 2, 0.0f);

                
            }
            else
            {
                coll.height = standingHeight;
                coll.center = new Vector3(coll.center.x, coll.center.y + crouchingHeight / 2, coll.center.z);
                playerHideCheck.localPosition = new Vector3(0.0f, 0.0f / 2, 0.0f);
            }

            anim.SetBool("Crouched", crouched);
        }
    }

    public void Die(bool overrideHide = false)
    {
        if (!hidden || overrideHide)
        {
            // walk.Stop();
            if (!deathSound.isPlaying)
            {
                deathSound.Play();
            }
            dead = true;
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            anim.SetTrigger("Dead");
            deathTransition.StartFade();
        }
    }

    public void Hide(bool hide)
    {
        hidden = hide;

        if (hidden)
        {
            anim.runtimeAnimatorController = animControllerHidden;
            anim.SetBool("Crouched", crouched);
        }
        else
        {
            anim.runtimeAnimatorController = animControllerVisible;
        }
    }

    IEnumerator MoveLayer()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y, layerGap * layer);
        float startTime = Time.time;

        int counter = 0;
        while (Vector3.Distance(transform.position, endPos) > 0.001 && counter < 1000)
        {
            transform.position = Vector3.Lerp(startPos, endPos, layerSpeed * (Time.time - startTime));
            counter++;

            yield return new WaitForEndOfFrame();
        }
    }
}