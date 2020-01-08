using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    private AudioSource _audioSource;
    private string _levelName;
    private bool startPlay = false;
    private bool doNotStop = false;

    void Awake()
    {
        _levelName = Application.loadedLevelName;
    }

    void Update()
    {
        if (_levelName != Application.loadedLevelName && Application.loadedLevelName!="Loading")
        {
            _levelName = Application.loadedLevelName;
            if (!doNotStop)
                audioSource.Stop();
        }
        if (startPlay)
        {
            audioSource.Play();
            startPlay = false;
        }
    }

    public bool isPlaying()
    {
        return audioSource.isPlaying;
    }

    public void playWhileLoading()
    {
        doNotStop = true;
    }

    public void doNotPlayWhileLoading()
    {
        doNotStop = false;
    }

    private AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
            {
                _audioSource = _instance.gameObject.AddComponent<AudioSource>();
                _audioSource.loop = true;
            }
            return _audioSource;
        }
    }

    public static MusicManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<MusicManager>();
                _instance.name = "MusicManager";
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip[] clip)
    {
        audioSource.PlayOneShot(clip[Random.Range(0, clip.Length)]);
    }

    public void PlayBackground(AudioClip clip)
    {
        if (clip == audioSource.clip&&audioSource.isPlaying) return;
        audioSource.Stop();
        audioSource.clip = clip;
        startPlay = true;
    }

    public void PlayBackground(AudioClip[] clip)
    {
        audioSource.Stop();
        audioSource.clip = clip[Random.Range(0, clip.Length)];
        startPlay = true;
    }

    public void StopBackground()
    {
        audioSource.Stop();
    }
}
