using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // movement params
    public float playerSpeed;
    private bool dead = false;
    private bool canMove = true;

    // components
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Animator anim;
    public CapsuleCollider coll;
    public LevelTransition deathTransition;

    // layer params
    public float layerGap = 1.8f;
    public int layer = 0;
    private bool movingLanes = false;
    public Transform layerCheckStart;
    public float layerSpeed;

    // crouching params
    bool crouched = false;
    float standingHeight;
    float crouchingHeight;

    // player is in shadow, cannot be seen by enemies
    public bool hidden = false;
    public Transform playerHideCheck;

    // animation controllers to use whether hidden or visible
    public RuntimeAnimatorController animControllerVisible;
    public RuntimeAnimatorController animControllerHidden;

    // audio
    public AudioSource walk;
    public AudioSource crouch;
    public AudioSource layerUp;
    public AudioSource layerDown;
    public AudioSource deathSound;

    Music bgMusic;

    // attacking params
    public bool hasKnife = false;
    public float attackRange = 2.0f;

    public Bossfight bossfight;

    private Tutorial dialogue;
    private BossfightCutsceneTrigger bossDialogue;
    private bool inDialogue = false;

    public bool finalAnim = false;

    void Start()
    {
        if (finalAnim)
        {
            // TODO: hug?
            // anim.SetTrigger();

            canMove = false;
            return;
        }

        rb = gameObject.GetComponent<Rigidbody>();
        coll = gameObject.GetComponent<CapsuleCollider>();

        bgMusic = FindObjectOfType<Music>();

        standingHeight = coll.height;
        crouchingHeight = standingHeight / 2.0f;
    }

    void Update()
    {
        if (dead || !canMove) return;

        if (Input.GetKeyDown(KeyCode.Space) && inDialogue)
        {
            if (bossDialogue)
            {
                bossDialogue.HideText();
                // TODO: maybe coroutine here like stab cutscene? except cutting through rope or doing something to provoke bossfight
                inDialogue = false;
                bossfight.NextPhase();
                return;
            }

            dialogue.HideText();
            if (dialogue.stab)
            {
                StartCoroutine(StabCutscene());
            }
            else
            {
                inDialogue = false;
            }
        }
        
        if (inDialogue) return;

        float x = Input.GetAxis("Horizontal");

        if (!crouched)
        {
            // walking sfx
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
            if (Input.GetKeyDown(KeyCode.W) && layer < 2 && !movingLanes)
            {
                RaycastHit hit;
                if (Physics.Raycast(layerCheckStart.position, transform.TransformDirection(Vector3.forward), out hit, 2))
                {
                    // Something is in way
                    if (hit.transform.tag == "Enemy")
                    {
                        movingLanes = true;
                        StartMoveLayer(1);
                    }
                }
                else
                {
                    movingLanes = true;
                    StartMoveLayer(1);
                }
            }
            if (Input.GetKeyDown(KeyCode.S) && layer > 0 && !movingLanes)
            {
                RaycastHit hit;
                if (Physics.Raycast(layerCheckStart.position, transform.TransformDirection(-Vector3.forward), out hit, 2))
                {
                    // Something is in way

                    if (hit.transform.tag == "Enemy")
                    {
                        movingLanes = true;
                        StartMoveLayer(-1);
                    }
                }
                else
                {
                    movingLanes = true;
                    StartMoveLayer(-1);
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
        if (Input.GetKeyDown(KeyCode.LeftControl))
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
            Debug.Log("Set Crouched");
        }
        
        // Attack
        Debug.DrawRay(transform.position, transform.TransformDirection(sr.flipX ? Vector3.left : Vector3.right) * attackRange, Color.green);
        if (Input.GetKeyDown(KeyCode.Space) && hasKnife)
        {
            anim.SetTrigger("Slash");
            Debug.Log("tried to cut");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(sr.flipX ? Vector3.left : Vector3.right), out hit, attackRange))
            {
                Debug.Log(hit.transform.tag);
                // TODO: rope tag
                if (hit.transform.tag == "Rope")
                {
                    Debug.Log("attack hit!");
                    Destroy(hit.transform.gameObject);
                    if (bossfight)
                    {
                        bossfight.EnemyDestroyed(hit.transform.gameObject);
                    }
                }
                else if (hit.transform.tag == "Enemy")
                {
                    if (hit.transform.gameObject.GetComponent<Enemy>().bossStunned == true)
                    {
                        Debug.Log("attack hit!");
                        Destroy(hit.transform.gameObject);
                        if (bossfight)
                        {
                            bossfight.EnemyDestroyed(hit.transform.gameObject);
                        }
                    }
                }
            }
        }
    }

    // 1 = move up layer, -1 = move down layer
    private void StartMoveLayer(int direction)
    {
        if (direction == 1)
        {
            layerUp.Play();
        }
        else
        {
            layerDown.Play();
        }
        layer += direction;
        anim.SetTrigger("MoveLayer");
        StartCoroutine(MoveLayer());
    }

    public void Die(bool overrideHide = false)
    {
        if (!hidden || overrideHide)
        {
            Enemy[] allHammerheads = FindObjectsOfType<Enemy>();

            for (int i = 0; i < allHammerheads.Length; i++)
            {
                allHammerheads[i].audioSource.Pause();
            }

            if (bgMusic)
            {
                bgMusic.PauseMusic();
            }
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
            bool uncrouching = false;

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Main_Uncrouch_Hidden"))
            {
                uncrouching = true;
            }

            anim.runtimeAnimatorController = animControllerVisible;

            if (uncrouching) // Uncrouches character when changing animators after uncrouching
            {
                anim.SetTrigger("Uncrouch");
            }
        }
    }

    // tutorial dialogue
    public void DialogueStart(Tutorial dialogue)
    {
        this.dialogue = dialogue;
        anim.SetFloat("Speed", 0.0f);
        inDialogue = true;
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // bossfight dialogue
    public void DialogueStart(BossfightCutsceneTrigger dialogue)
    {
        this.bossDialogue = dialogue;
        anim.SetFloat("Speed", 0.0f);
        inDialogue = true;
        StartMoveLayer(1 - layer);
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void StartBow()
    {
        canMove = false;
        anim.SetFloat("Speed", 0.0f);
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        StartMoveLayer(0 - layer);
        StartCoroutine(Bow());
    }

    IEnumerator Bow()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetTrigger("Bow");
        // TODO: applause?
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
        movingLanes = false;
    }

    IEnumerator StabCutscene()
    {
        canMove = false;
        sr.flipX = true;
        anim.SetTrigger("StabCutscene");
        yield return new WaitForSeconds(12.5f); // magic number
        canMove = true;
        inDialogue = false;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(transform.position, transform.position + (sr.flipX ? Vector3.left : Vector3.right) * attackRange);
    }
}