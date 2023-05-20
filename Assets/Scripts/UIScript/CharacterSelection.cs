using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public int selectedCharacter = 0;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;

    void Start()
    {
        CharacterData();

    }
    public void NextCharacter()
    {
        if (selectedCharacter < 2)
        {
            characters[selectedCharacter].SetActive(false);
            selectedCharacter = selectedCharacter + 1 % characters.Length;
            characters[selectedCharacter].SetActive(true);
            CharacterData();
        }
    }
    
    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if(selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
        CharacterData();
    }

    void CharacterData()
    {
        HealthDamage data = characters[selectedCharacter].GetComponent<HealthDamage>();
        healthText.text = "Health: " + data.health.ToString();
        damageText.text = "Damage: " + data.damage.ToString();
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene("Main Game 1");
    }
}
