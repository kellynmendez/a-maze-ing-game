using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{

    [SerializeField] List<GameObject> locks;
    [SerializeField] Collider openCollider;

    [Header("Feedback")]
    [SerializeField] AudioClip collectSFX = null;
    //[SerializeField] ParticleSystem collectParticle = null;

    private PlayerController player;
    private AudioSource audioSource = null;

    private void Awake()
    {
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.None)[0];
        audioSource = GetComponent<AudioSource>();
        openCollider.enabled = false;
    }
    public void OpenDoor()
    {
        openCollider.enabled = true;
        PlayOpenFX();

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("GAME WON!");
        player.GameWon();
    }

    public void KeyCollected()
    {
        foreach (GameObject go in locks)
        {
            if (go.activeSelf)
            {
                go.SetActive(false);
                break;
            }
        }
    }

    void PlayOpenFX()
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
