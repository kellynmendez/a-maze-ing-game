using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Carrot : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] AudioClip collectSFX = null;

    [Header("Required References")]
    [SerializeField] Collider triggerToDisable = null;
    [SerializeField] GameObject artToDisable = null;

    AudioSource audioSource = null;
    Timer timer;

    private void Awake()
    {
        timer = FindObjectsByType<Timer>(FindObjectsSortMode.None)[0];
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerToDisable.enabled = false;
        artToDisable.SetActive(false);
        timer.AddTime();
        PlayFX();
    }

    void PlayFX()
    {
        if (audioSource != null && collectSFX != null)
        {
            audioSource.PlayOneShot(collectSFX, audioSource.volume);
        }
    }
}