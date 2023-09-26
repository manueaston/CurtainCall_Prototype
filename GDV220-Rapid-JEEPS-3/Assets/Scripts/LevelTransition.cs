using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public GameObject curtains;
    public string nextScene;
    public bool disabled = false;

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

    public void StartFade()
    {
        Debug.Log("finished level");
        StartCoroutine(OnCurtain());
    }

    // wait for curtain to end (around 1.5s) then load next scene
    IEnumerator OnCurtain()
    {
        yield return new WaitForSeconds(0.75f);
        curtains.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nextScene);
    }
}