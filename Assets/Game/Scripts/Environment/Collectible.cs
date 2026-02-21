using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    /*[SerializeField] Collider _triggerToDisable = null;
    [SerializeField] GameObject _artToDisable = null;

    [Header("Feedback")]
    [SerializeField] AudioClip _collectibleSFX = null;
    [SerializeField] ParticleSystem _collectibleParticle = null;

    AudioSource _audioSource = null;
    int _collectScoreIncr = 60;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collectible has been entered!");
        *//*PlayerShip _playerShip = other.attachedRigidbody.GetComponent<PlayerShip>();
        if (_playerShip != null)
        {
            _playerShip.UpdateScore(_collectScoreIncr);
            PlayFX();
        }*//*

        // disable relevant components for "Collection"
        Debug.Log("Disabling...");
        _triggerToDisable.enabled = false;
        _artToDisable.SetActive(false);
    }

    void PlayFX()
    {
        // play gfx
        if (_collectibleParticle != null)
        {
            _collectibleParticle.Play();
        }
        // play sfx
        if (_audioSource != null && _collectibleSFX != null)
        {
            _audioSource.PlayOneShot(_collectibleSFX, _audioSource.volume);
        }
    }*/
}
