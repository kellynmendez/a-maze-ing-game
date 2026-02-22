using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem.XR;

public class GameManager : MonoBehaviour
{
    PlayerController player;
    AudioManager audioMgr;
    UIController uiController;
    bool checkPlayerDead = false;
    bool checkPlayerWon = false;

    private void Awake()
    {
        player = FindObjectsByType<PlayerController>(FindObjectsSortMode.None)[0];
        audioMgr = FindObjectsByType<AudioManager>(FindObjectsSortMode.None)[0];
        uiController = FindObjectsByType<UIController>(FindObjectsSortMode.None)[0];
    }

    void Update()
    {
        if (!checkPlayerDead && player.IsDead())
        {
            checkPlayerDead = player.IsDead();
            uiController.ShowLoseText();
            Time.timeScale = 0;
            StartCoroutine(MoveToCredits());
        }
        else if (!checkPlayerWon && player.CheckWon())
        {
            checkPlayerWon = player.CheckWon();
            uiController.ShowWinText();
            StartCoroutine(MoveToCredits());
            Time.timeScale = 0;
        }
    }

    public void ExitLevel()
    {
        Time.timeScale = 1;
        audioMgr.GoingToCredits();
        SceneManager.LoadScene("Credits");
    }

    IEnumerator MoveToCredits()
    {
        yield return new WaitForSecondsRealtime(3f);
        ExitLevel();
    }
}
