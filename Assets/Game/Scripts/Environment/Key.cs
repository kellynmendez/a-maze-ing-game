using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Key : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] AudioClip collectSFX = null;
    //[SerializeField] ParticleSystem collectParticle = null;

    [Header("Required References")]
    [SerializeField] Collider triggerToDisable = null;
    [SerializeField] GameObject artToDisable = null;

    AudioSource audioSource = null;
    PlayerCollection player;

    private void Awake()
    {
        player = FindObjectsByType<PlayerCollection>(FindObjectsSortMode.None)[0];
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        player.AddKey();
        triggerToDisable.enabled = false;
        artToDisable.SetActive(false);
        PlayFX();
    }

    void PlayFX()
    {
        // play gfx
        /*if (collectParticle != null)
        {
            collectParticle.Play();
        }*/
        // play sfx
        if (audioSource != null && collectSFX != null)
        {
            audioSource.PlayOneShot(collectSFX, audioSource.volume);
        }
    }
}