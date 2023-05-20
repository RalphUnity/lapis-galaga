using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public static LoadCharacter Instance;

    public GameObject[] characterPrefabs;
    public Transform spawnPoint;
    [HideInInspector]
    public GameObject loadCharObj;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        loadCharObj = characterPrefabs[selectedCharacter];
        loadCharObj.name = "PlayerShip"; 
        Instantiate(loadCharObj, spawnPoint.position, Quaternion.identity);
    }

}
