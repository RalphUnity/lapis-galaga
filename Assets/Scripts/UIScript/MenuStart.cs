using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
    public TextMeshProUGUI highestScoreText;
    public TextMeshProUGUI highestLevelText;

    void Start()
    {
        highestScoreText.text = "Highest Score: " + PlayerPrefs.GetInt("HighScore");
        highestLevelText.text = "Highest Level: " + PlayerPrefs.GetInt("HighestLevel");
    }

    public void ClickStart()
    {
        SceneManager.LoadScene("Character Selection");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
