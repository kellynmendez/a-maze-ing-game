using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;
    [SerializeField] List<GameObject> keys;

    private int numKeys = 0;
    private void Awake()
    {
        HideWinText();
        HideLoseText();
    }

    private void Start()
    {
        foreach (GameObject key in keys)
        {
            key.SetActive(false);
        }
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

    public void AddKey()
    {
        keys[numKeys].SetActive(true);
        numKeys++;
    }
}
