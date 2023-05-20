using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{

    public static UIScript Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI stageText;
    public Button fireButton;

    // Fire button cooldown
    public Image fireImage;
    public float cooldown = 2f;
    bool isCooldown = false;
    bool isPress = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isPress)
        {
            Fire();
        }
    }

    public void IsClick()
    {
        isPress = true;
    }

    public void Fire()
    {
        if (!isCooldown)
        {
            isCooldown = true;
            fireImage.fillAmount = 1;
            Player.Instance.Fire();
        }

        if (isCooldown)
        {
            fireImage.fillAmount -= 1 / cooldown * Time.deltaTime;

            if(fireImage.fillAmount <= 0)
            {
                fireImage.fillAmount = 0;
                isCooldown = false;
                isPress = false;
            }
        }
    }

    public void UpdateScoreText(int amount)
    {
        scoreText.text = amount.ToString("D9");
    }

    public void UpdateHealthText(int amount)
    {
        healthText.text = amount.ToString("D1");
    }
    public void ShowStageText(int amount)
    {
        stageText.gameObject.SetActive(true);
        stageText.text = "Stage " + amount;

        Invoke("DeactivateStageText", 3f);
    }

    void DeactivateStageText()
    {
        stageText.gameObject.SetActive(false);
        CancelInvoke("DeactivateStageText");
    }
}
