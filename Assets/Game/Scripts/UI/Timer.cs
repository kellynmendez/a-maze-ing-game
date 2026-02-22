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
    //UIController _uiController = null;
    private bool playerDead = false;
    Color startColor;

    private void Awake()
    {
        startColor = timerText.color;
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

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // When time is getting low, make timer more noticeable
        if (timeToDisplay < 10)
        {
            timerText.color = Color.Lerp(startColor, Color.red, Mathf.PingPong(Time.time, 1));
        }
    }

    void EndGame()
    {
        //_uiController.ShowText(loseText);
        player.Kill();
    }
}
