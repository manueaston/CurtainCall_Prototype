using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public GameObject curtains;
    public bool useBuildOrder; // disable for final scene & any necessary diversions
    public bool restartScene;
    public string nextScene;
    public bool disabled = false;
    public Image image;

    float curtainDelayTime = 0.75f;
    float curtainAnimTime = 1.5f;

    public Music bgMusic;

    private void Start()
    {
        bgMusic = FindObjectOfType<Music>();
    }

    // if player collides with object, load next scene
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        if (!disabled)
        {
            if (other.gameObject.tag == "Player")
            {
                StartFade();
            }
        }
    }

    public void StartFade(bool white = false)
    {
        Debug.Log("finished level");
        if (bgMusic)
        {
            StartCoroutine(bgMusic.Fade(bgMusic.maxVolume, 0, curtainDelayTime + curtainAnimTime));
        }
        StartCoroutine(OnCurtain());
    }

    // wait for curtain to end (around 1.5s) then load next scene
    IEnumerator OnCurtain(bool white = false)
    {
        if (white)
        {
            image.color = Color.white;
        }

        yield return new WaitForSeconds(curtainDelayTime);
        curtains.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(curtainAnimTime);

        if (restartScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // load scene index + 1
        }
        else if (useBuildOrder)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // load scene index + 1
        }
        else
        {
            SceneManager.LoadScene(nextScene); // load specified nextScene
        }
    }
}