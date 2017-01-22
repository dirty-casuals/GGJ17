using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] songs;
    private const string objectName = "SoundManager";
    private const float songDuration = 60.0f;
    private static SoundManager _instance;
    private AudioSource audioSource;
    private float songTimer;
    private int songCount = 1;

    private void Awake()
    {
        if( _instance == null )
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }

        if( _instance != null && Instance != this )
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
        songTimer = songDuration;
    }

    private void Update()
    {
        songTimer -= Time.deltaTime;
        if( songTimer < 0.0f )
        {
            PlayRandomSong();
            songTimer = songDuration;
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if( _instance == null )
            {
                _instance = new GameObject(objectName).AddComponent<SoundManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private void PlayRandomSong()
    {
        if( songCount >= songs.Length )
        {
            songCount = 0;
        }
        StartCoroutine(FadeMusicInAndOut(songs[songCount]));
        songCount += 1;
    }

    private IEnumerator FadeMusicInAndOut(AudioClip newSong)
    {
        yield return StartCoroutine(FadeMusicOut());
        audioSource.clip = newSong;
        yield return new WaitForSeconds(1.0f);
        audioSource.Play();
        yield return StartCoroutine(FadeMusicIn());
    }

    private IEnumerator FadeMusicOut()
    {
        while( audioSource.volume > 0.01 )
        {
            audioSource.volume -= 0.009f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator FadeMusicIn()
    {
        while( audioSource.volume < 0.5f )
        {
            audioSource.volume += 0.009f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
