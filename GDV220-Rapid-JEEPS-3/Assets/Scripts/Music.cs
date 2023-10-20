using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    // Start is called before the first frame update

    public enum ActMusic
    { 
        TitleScreen,
        Act1,
        Act2,
        Act3,
        Boss
    }

    public ActMusic musicTrack = new ActMusic();

    public float maxVolume;

    public bool fadeMusicIn;
    public float timeToFade;

    public AudioClip MusicTitleScreen;
    public AudioClip MusicAct1;
    public AudioClip MusicAct2;
    public AudioClip MusicAct3;
    public AudioClip MusicBoss;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Select music based on level
        switch (musicTrack)
        {
            case ActMusic.TitleScreen:
                audioSource.clip = MusicTitleScreen;
                break;
            case ActMusic.Act1:
                audioSource.clip = MusicAct1;
                break;
            case ActMusic.Act2:
                audioSource.clip = MusicAct2;
                break;
            case ActMusic.Act3:
                audioSource.clip = MusicAct3;
                break;
            case ActMusic.Boss:
                audioSource.clip = MusicBoss;
                break;
        }
        audioSource.Play();

        if (fadeMusicIn)
        {
            StartCoroutine(Fade(0, maxVolume, timeToFade));
        }
        else
        {
            audioSource.volume = maxVolume;
        }
    }

    public IEnumerator Fade(float startVolume, float endVolume, float duration)
    {
        audioSource.volume = startVolume;
        float currentTime = 0;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(startVolume, endVolume, currentTime / duration);
            yield return null;
        }

        audioSource.volume = endVolume;
        yield break;
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }
}
