using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinVolume : MonoBehaviour
{
    [SerializeField] AudioClip winSFX = null;

    //UIController uIController = null;
    AudioSource audioSource = null;

    private void Awake()
    {
        // Searching objects in scene for script of type UIController, then storing
        //uIController = FindObjectOfType<UIController>();

        // Searching for audio source component
        audioSource = GetComponent<AudioSource>();

    }

    private void OnTriggerEnter(Collider other)
    {
        // Turn off gameObject that entered
        other.gameObject.SetActive(false);
        // If controller was found
        /*if (uIController != null)
        {
            // Show text on win
            PlayFeedback();
        }*/
    }

    void PlayFeedback()
    {
        if (audioSource != null && winSFX != null)
        {
            audioSource.clip = winSFX;
            audioSource.Play();
        }
    }
}