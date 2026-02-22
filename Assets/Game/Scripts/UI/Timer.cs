using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] float timeValue = 30f;
    [SerializeField] TMP_Text timerText;
    [SerializeField] Animation timerAnim;
    //UIController _uiController = null;
    private bool playerDead = false;

    private void Awake()
    {
        // Searching objects in scene for script of type UIController
        //_uiController = FindObjectOfType<UIController>();
    }

    void Update()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            playerDead = player.IsDead();
            if (!playerDead)
            {
                timerAnim.Play();
                EndGame();
                timeValue = 0;
            }
        }

        DisplayTime(timeValue);
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        timerText.text = string.Format("00:{0:00}", timeToDisplay);

        // When time is getting low, make timer more noticeable
        if (timeToDisplay < 10)
        {
            timerText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));
        }
    }

    void EndGame()
    {
        //_uiController.ShowText(loseText);
        player.Kill();
    }
}
