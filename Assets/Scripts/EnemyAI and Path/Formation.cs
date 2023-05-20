using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Formation : MonoBehaviour
{
    public int gridSizeX = 10;
    public int gridSizeZ = 2;

    public float gridOffsetX = 1f;
    public float gridOffsetZ = 1f;

    public int div = 4;

    public List<Vector3> gridList = new List<Vector3>();

    // Move the formation
    public float maxMoveOffsetX = 5f;

    float curPosX; // moving position
    Vector3 startPosition;

    public float speed = 1f;
    int direction = -1;

    // Spreading 
    bool canSpread;
    bool spreadStarted;

    float spreadAmount = 1f;
    float curSpread;
    float spreadSpeed = 0.5f;
    int spreadDir = 1;

    // Diving
    bool canDive;
    public List<GameObject> divePathList = new List<GameObject>();

    [HideInInspector]
    public List<EnemyFormation> enemyList = new List<EnemyFormation>();

    [System.Serializable]
    public class EnemyFormation
    {
        public int index;
        public float xPos;
        public float zPos;
        public GameObject enemy;

        public Vector3 start;
        public Vector3 goal;

        public EnemyFormation(int _index, float _xPos, float _zPos, GameObject _enemy)
        {
            index = _index;
            xPos = _xPos;
            zPos = _zPos;
            enemy = _enemy;

            start = new Vector3(_xPos, 0, _zPos);
            goal = new Vector3(xPos + (xPos * 0.3f), 0, zPos);
        }
    }

    void Start()
    {
        startPosition = transform.position;
        curPosX = transform.position.x;

        CreateGrid();
    }

    void Update()
    {
        if (!canSpread && !spreadStarted)
        {
            curPosX += Time.deltaTime * speed * direction;
            if (curPosX >= maxMoveOffsetX)
            {
                direction *= -1;
                curPosX = maxMoveOffsetX;
            }
            else if (curPosX <= -maxMoveOffsetX)
            {
                direction *= -1;
                curPosX = -maxMoveOffsetX;
            }
            transform.position = new Vector3(curPosX, startPosition.y, startPosition.z);
        }

        if (canSpread)
        {
            curSpread += Time.deltaTime * spreadDir * spreadSpeed;
            if(curSpread >= spreadAmount || curSpread <= 0)
            {
                // Change spread direction
                spreadDir *= -1;
            }

            for (int i = 0; i < enemyList.Count; i++)
            {
                float dist = Vector3.Distance(enemyList[i].enemy.transform.position, enemyList[i].goal);
                if (dist >= 0.001f)
                {
                    enemyList[i].enemy.transform.position = Vector3.Lerp(transform.position + enemyList[i].start,
                        transform.position + enemyList[i].goal, curSpread);
                }
            }
        }
    }

    public IEnumerator ActivateEnemySpread()
    {
        if (spreadStarted)
        {
            yield break;
        }
        spreadStarted = true;

        while(transform.position.x != startPosition.x)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            yield return null;
        }
        canSpread = true;
        //canDive = true;
        Invoke("SetDiving", Random.Range(3, 10));
    }

    private void OnDrawGizmos()
    {
        int num = 0;

        CreateGrid();
        foreach (Vector3 pos in gridList)
        {
            Gizmos.DrawWireSphere(GetVector(num), 0.1f);
            num++;
        }
    }

    void CreateGrid()
    {
        gridList.Clear();

        int num = 0;

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                float x = (gridOffsetX + gridOffsetX * 2 * (num / div)) * Mathf.Pow(-1, num % 2 + 1);
                float z = gridOffsetZ * ((num % div) / 2);

                Vector3 vec = new Vector3(x, 0, z);
                num++;

                gridList.Add(vec);
            }
        }
    }

    public Vector3 GetVector(int ID)
    {
        return transform.position + gridList[ID];
    }

    void SetDiving()
    {
        if (enemyList.Count > 0)
        {
            int choosenPath = Random.Range(0, divePathList.Count);
            int choosenEnemy = Random.Range(0, enemyList.Count);

            GameObject newPath = Instantiate(divePathList[choosenPath],
                                             enemyList[choosenEnemy].start + transform.position,
                                             Quaternion.identity);

            EnemyBehaviour behaviour = enemyList[choosenEnemy].enemy.GetComponent<EnemyBehaviour>();
            behaviour.DiveSetup(newPath.GetComponent<Path>());
            enemyList.RemoveAt(choosenEnemy);
            Invoke("SetDiving", Random.Range(3, 10));
        }
        else
        {
            CancelInvoke("SetDiving");
        }
    }
}
