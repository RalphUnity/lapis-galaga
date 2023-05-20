using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: " + ScoreHolder.score;
        levelText.text = "Highest Level: " + ScoreHolder.level;
        if (ScoreHolder.score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", ScoreHolder.score);
            PlayerPrefs.SetInt("HighestLevel", ScoreHolder.level);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
