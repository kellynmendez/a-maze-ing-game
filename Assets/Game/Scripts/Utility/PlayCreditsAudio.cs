using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayCreditsAudio : MonoBehaviour
{
    AudioManager audioMgr;
    private void Start()
    {
        audioMgr = FindObjectsByType<AudioManager>(FindObjectsSortMode.None)[0];
        audioMgr.PlayCredits();
    }
}
