using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;
    private void Awake()
    {
        HideWinText();
        HideLoseText();
    }
    public void ShowWinText()
    {
        winText.SetActive(true);
    }

    public void HideWinText()
    {
        winText.SetActive(false);
    }

    public void ShowLoseText()
    {
        loseText.SetActive(true);
    }

    public void HideLoseText()
    {
        loseText.SetActive(false);
    }

}
