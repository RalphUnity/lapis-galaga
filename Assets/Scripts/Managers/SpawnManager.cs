using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Intervals")]
    public float enemySpawnInterval; // interval between ship spawns
    public float waveSpawnInterval; // interval between waves
    int currentWave;

    int flyID = 0;
    int waspID = 0;
    int bossID = 0;

    [Header("Enemy Formation")]
    public Formation flyFormation;
    public Formation waspFormation;
    public Formation bossFormation;

    [System.Serializable]
    public class Wave
    {
        public int flyAmount;
        public int waspAmount;
        public int bossAmount;

        public GameObject[] pathPrefabs;
    }

    [Header("Waves")]
    public List<Wave> waveList = new List<Wave>();

    List<Path> activePathList = new List<Path>();

    [HideInInspector]
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    bool spawnComplete;

    ObjectPooler objectPooler;

    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        Invoke("StartSpawn", 3f);
    }

    IEnumerator SpawnWaves()
    {
        while(currentWave < waveList.Count)
        {
            if (currentWave == waveList.Count - 1)
            {
                spawnComplete = true;
            }

            for (int i = 0; i < waveList[currentWave].pathPrefabs.Length; i++)
            {
                // No need to object pool since we instantiate different path objects
                GameObject newPathObj = Instantiate(waveList[currentWave].pathPrefabs[i],
                    transform.position, Quaternion.identity);
                Path path = newPathObj.GetComponent<Path>();
                activePathList.Add(path);
            }

            // To spawn flies
            for (int i = 0; i < waveList[currentWave].flyAmount; i++)
            {
                // Spawn flies from object pooler
                GameObject newFly = objectPooler.SpawnFromPool("Fly", transform.position, Quaternion.identity);
                EnemyBehaviour flyBehaviour = newFly.GetComponent<EnemyBehaviour>();

                flyBehaviour.SpawnSetup(activePathList[PathPingPong()], flyID, flyFormation);
                flyID++;

                spawnedEnemies.Add(newFly);

                // Wait for spawn interval
                yield return new WaitForSeconds(enemySpawnInterval);
            }

            // To spawn wasps
            for (int i = 0; i < waveList[currentWave].waspAmount; i++)
            {
                // Spawn wasp from object pooler
                GameObject newWasp = objectPooler.SpawnFromPool("Wasp", transform.position, Quaternion.identity);
                EnemyBehaviour waspBehaviour = newWasp.GetComponent<EnemyBehaviour>();

                waspBehaviour.SpawnSetup(activePathList[PathPingPong()], waspID, waspFormation);
                waspID++;

                spawnedEnemies.Add(newWasp);

                // Wait for spawn interval
                yield return new WaitForSeconds(enemySpawnInterval);
            }

            // To spawn bosses
            for (int i = 0; i < waveList[currentWave].bossAmount; i++)
            {
                // Spawn boss from object pooler
                GameObject newBoss = objectPooler.SpawnFromPool("Boss", transform.position, Quaternion.identity);
                EnemyBehaviour bossBehaviour = newBoss.GetComponent<EnemyBehaviour>();

                bossBehaviour.SpawnSetup(activePathList[PathPingPong()], bossID, bossFormation);
                bossID++;

                spawnedEnemies.Add(newBoss);

                // Wait for spawn interval
                yield return new WaitForSeconds(enemySpawnInterval);
            }

            yield return new WaitForSeconds(waveSpawnInterval);
            currentWave++;

            foreach (Path path in activePathList)
            {
                Destroy(path.gameObject);
            }
            activePathList.Clear();
        }

        Invoke("CheckEnemyState", 1f);
    }

    void CheckEnemyState()
    {
        bool inFormation = false;
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            EnemyBehaviour enemyBehaviour = spawnedEnemies[i].GetComponent<EnemyBehaviour>();
            if (enemyBehaviour.enemyState != EnemyBehaviour.EnemyStates.IDLE)
            {
                inFormation = false;
                Invoke("CheckEnemyState", 1f);
                break;
            }
        }
        inFormation = true;

        if (inFormation)
        {
            StartCoroutine(flyFormation.ActivateEnemySpread());
            StartCoroutine(waspFormation.ActivateEnemySpread());
            StartCoroutine(bossFormation.ActivateEnemySpread());
            CancelInvoke("CheckEnemyState");
        }
    }

    void StartSpawn()
    {
        StartCoroutine(SpawnWaves());
        CancelInvoke("StartSpawn");
    }

    /// <summary>
    /// Get enemy path ID
    /// </summary>
    int PathPingPong()
    {
        return (flyID + waspID + bossID) % activePathList.Count;
    }

    // To not overdo enemy count
    void OnValidate()
    {
        // FLIES
        int currentFlyAmount = 0;
        for (int i = 0; i < waveList.Count; i++)
        {
            currentFlyAmount += waveList[i].flyAmount;
        }
        
        if(currentFlyAmount > 20)
        {
            Debug.LogError("Your fly amount is too high! " + currentFlyAmount + "/ 20");
        }
        else
        {
            Debug.LogError("Current Total Fly Amount: " + currentFlyAmount);
        }

        // Wasp
        int currentWaspAmount = 0;
        for (int i = 0; i < waveList.Count; i++)
        {
            currentWaspAmount += waveList[i].waspAmount;
        }

        if (currentWaspAmount > 16)
        {
            Debug.LogError("Your Wasp amount is too high! " + currentWaspAmount + "/ 16");
        }
        else
        {
            Debug.LogError("Current Total Wasp Amount: " + currentWaspAmount);
        }

        // Bosses
        int currentBossAmount = 0;
        for (int i = 0; i < waveList.Count; i++)
        {
            currentBossAmount += waveList[i].bossAmount;
        }

        if (currentBossAmount > 4)
        {
            Debug.LogError("Your Boss amount is too high! " + currentBossAmount + "/ 4");
        }
        else
        {
            Debug.LogError("Current Total Boss Amount: " + currentBossAmount);
        }
    }

    void ReportToGameManager()
    {
        if(spawnedEnemies.Count == 0 && spawnComplete)
        {
            GameManager.Instance.WinCondition();
        }
    }

    public void UpdateSpawnedEnemies(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        ReportToGameManager();
    }
}
