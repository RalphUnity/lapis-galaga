using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    static int level = 1; //Current Level
    static int score;
    public int health;

    int scoreToBonusLife = 10000;

    static int bonusScore;
    static bool hasLost;

    // Score
    /* 
     *  Flys in formation = 50 score, flying around = 100 score
     *  Wasps in formation = 80 score, flying around = 160
     *  Boss in formation =  100 score, flying around = 400
     */

    void Awake()
    {
        Instance = this;

        if (hasLost)
        {
            level = 1;
            score = 0;
            bonusScore = 0;
            hasLost = false;
        }
    }

    void Start()
    {
        // Set initial values
        UIScript.Instance.UpdateScoreText(score);
        UIScript.Instance.UpdateHealthText(health);
        UIScript.Instance.ShowStageText(level);
    }

    public void AddScore(int amount)
    {
        score += amount;

        UIScript.Instance.UpdateScoreText(score);

        Debug.LogError(score);
        bonusScore += amount;

        if(bonusScore >= scoreToBonusLife)
        {
            bonusScore %= scoreToBonusLife;
        }
    }

    public void UpdateLife(int amount)
    {
        health -= amount;
        UIScript.Instance.UpdateHealthText(amount);
        if (health <= 0)
        {
            // Game Over
            ScoreHolder.level = level;
            ScoreHolder.score = score;
            hasLost = true;
            SceneManager.LoadScene("GameOver");
        }
    }

    public void WinCondition()
    {
        level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
