using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip levelClip;

    AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" ||
            SceneManager.GetActiveScene().name == "Credits")
        {
            PlayGameScore(mainMenuClip);
        }
        else
        {
            PlayGameScore(levelClip);
        }
    }

    public void PlayGameScore(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void GoingToCredits()
    {
        PlayGameScore(mainMenuClip);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {

                SceneManager.LoadScene("MainPlayScene");
                PlayGameScore(levelClip);
            }
            if (SceneManager.GetActiveScene().name == "Credits")
            {
                SceneManager.LoadScene("MainMenu");
                PlayGameScore(mainMenuClip);
            }
        }
    }
}