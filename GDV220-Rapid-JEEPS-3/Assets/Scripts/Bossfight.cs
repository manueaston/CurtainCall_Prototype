using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPhase
{
    Attack,
    Stunned
}

public class Bossfight : MonoBehaviour
{
    // components
    [SerializeField] private Animator lHandAnim, rHandAnim;
    [SerializeField] private Music music;

    // bossfight params
    [SerializeField] private GameObject player;
    public GameObject bossLight;
    [SerializeField] private Light stageLight;
    private bool hasStarted = false;
    private BossPhase phase = BossPhase.Attack;
    private int attackPhases = 0;
    public float speed;
    private bool makingHeadVisible = false;
    private bool headVisible = false;
    private bool gameOver = false;
    [SerializeField] private float visibleHeadHeight;
    [SerializeField] private float invisibleHeadHeight;
    [SerializeField] private float visibleEnemyHeight;
    [SerializeField] private float invisibleEnemyHeight;
    private static int maxSharks = 4;
    private static int maxFrogs = 4;
    [SerializeField] private Vector3[] sharkSpawnPos = new Vector3[12]; // 4 * 3 phases
    [SerializeField] private Vector3[] frogSpawnPos = new Vector3[maxFrogs];
    [SerializeField] private Vector3 flySpawnPos; // only one fly at a time
    private List<GameObject> enemies = new List<GameObject>();

    // enemy prefabs
    [SerializeField] private GameObject shark;
    [SerializeField] private GameObject frog;
    [SerializeField] private GameObject fly;

    // end transition
    [SerializeField] private LevelTransition door;

    public bool HasStarted()
    {
        return hasStarted;
    }

    public void NextPhase()
    {
        attackPhases++;

        // fourth phase: end fight
        if (attackPhases == 4)
        {
            StartCoroutine(EndGame());
            return;
        }

        lHandAnim.SetTrigger("Slam");
        rHandAnim.SetTrigger("Slam");
        // spawn new enemies - increase by 4 each phase
        for (int i = 0; i < maxSharks * attackPhases; i++)
        {
            if (sharkSpawnPos[i].x - 2.5f < player.transform.position.x && sharkSpawnPos[i].x + 2.5f > player.transform.position.x) continue; // if spawnpos overlaps player
            enemies.Add(Instantiate(shark, sharkSpawnPos[i], Quaternion.identity));
            enemies[enemies.Count - 1].GetComponent<Enemy>().IsBossfight();
            // enemies[enemies.Count - 1].GetComponent<Enemy>().ResetOriginalPos();
        }
        // only spawn frogs at start
        if (attackPhases == 1)
        {
            for (int i = 0; i < maxFrogs; i++)
            {
                enemies.Add(Instantiate(frog, frogSpawnPos[i], Quaternion.AngleAxis(180, Vector3.up)));
                enemies[enemies.Count - 1].GetComponent<MovingWatcherEnemy>().stationary = true;
            }
        }
        StartCoroutine(AddFly());

        hasStarted = true;
    }

    void Update()
    {
        if (gameOver)
        {
            transform.Translate(0.0f, -speed * 3.0f, 0.0f);
        }

        if (!hasStarted) return;

        // spawn new enemies
        // while (enemies.Count < maxEnemies && phase != BossPhase.Stunned)
        // {
        //     enemies.Add(Instantiate(
        //         phase == BossPhase.Attack_Shark ? shark : phase == BossPhase.Attack_Frog ? frog : fly,
        //         enemySpawnPos[Random.Range(0, maxEnemies - 1)], Quaternion.identity
        //     ));
        // }
        
        // make head visible
        if (!makingHeadVisible)
        {
            StartCoroutine(MakeHeadVisible());
            makingHeadVisible = true;
        }

        // move head based on visibility setting
        if (headVisible && transform.position.y > visibleHeadHeight)
        {
            transform.Translate(0.0f, -speed * Time.deltaTime, 0.0f);
        }
        else if (!headVisible && transform.position.y < invisibleHeadHeight)
        {
            transform.Translate(0.0f, speed * 3.0f * Time.deltaTime, 0.0f);
        }

        // move characters based on phase
        foreach (GameObject obj in enemies)
        {
            if (phase == BossPhase.Stunned)
            {
                lHandAnim.SetTrigger("Clench");
                rHandAnim.SetTrigger("Clench");
                // TODO: inheritance/polymorphism could simplify this
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.BossStunned();
                    continue;
                }
                MovingWatcherEnemy movingEnemy = obj.GetComponent<MovingWatcherEnemy>();
                if (movingEnemy)
                {
                    movingEnemy.BossStunned();
                    continue;
                }
                // obj.transform.Translate(0.0f, -speed, 0.0f);
            }
            else if (phase != BossPhase.Stunned)
            {
                // TODO: inheritance/polymorphism could simplify this
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy && obj.transform.position.y < visibleEnemyHeight)
                {
                    enemy.BossStunned(false);
                    continue;
                }
                MovingWatcherEnemy movingEnemy = obj.GetComponent<MovingWatcherEnemy>();
                if (movingEnemy)
                {
                    movingEnemy.BossStunned(false);
                    continue;
                }
                // obj.transform.Translate(0.0f, speed, 0.0f);
            }
        }

        // boss defeated - allow level to end
        if (enemies.Count <= maxFrogs + 1) // maxFrogs + 1 = frogs & fly, therefore all sharks defeated
        {
            door.disabled = false;
        }
    }

    public void Stun()
    {
        if (phase != BossPhase.Stunned && headVisible)
        {
            phase = BossPhase.Stunned;
            StartCoroutine(LeaveStun());
        }
    }

    public void EnemyDestroyed(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    IEnumerator AddFly()
    {
        yield return new WaitForSeconds(12);
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        // enemies.Add(Instantiate(fly, flySpawnPos, rotation));
    }

    IEnumerator MakeHeadVisible()
    {
        Debug.Log("BOSSFIGHT: starting head visible routine");
        yield return new WaitForSeconds(15);
        headVisible = true;
        makingHeadVisible = false;
    }

    IEnumerator LeaveStun()
    {
        bossLight.SetActive(false);
        stageLight.intensity = 2.0f;
        StartCoroutine(Flicker());
        StopCoroutine(MakeHeadVisible());
        makingHeadVisible = false;

        yield return new WaitForSeconds(5 * attackPhases);
        phase = BossPhase.Attack; // TODO: more random?
        headVisible = false;
        lHandAnim.ResetTrigger("Clench");
        rHandAnim.ResetTrigger("Clench");

        bool sharksRemaining = false;
        foreach (GameObject obj in enemies)
        {
            // if sharks still exists, continue with current phase
            if (obj.GetComponent<Enemy>())
            {
                Debug.Log("shark still exists - continue with same phase");
                sharksRemaining = true;
                break;
            }
        }
        if (!sharksRemaining)
        {
            Debug.Log("all sharks eliminated - go to next phase");
            NextPhase();
        }
    }

    IEnumerator Flicker()
    {
        yield return new WaitForSeconds(4.5f * attackPhases);
        bossLight.SetActive(true);
        stageLight.intensity = 11.23734f;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
        bossLight.SetActive(false);
        stageLight.intensity = 2.0f;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
        bossLight.SetActive(true);
        stageLight.intensity = 11.23734f;
    }

    IEnumerator EndGame()
    {
        StartCoroutine(music.Fade(0.1f, 0.0f, 0.5f));
        stageLight.intensity = 7.0f;
        bossLight.SetActive(false);

        phase = BossPhase.Stunned;
        gameOver = true;
        player.GetComponent<Player>().StartBow();
        yield return new WaitForSeconds(3.5f);
        door.StartFade(true);
    }
}