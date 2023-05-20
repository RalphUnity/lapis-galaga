using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour, IStats
{
    // Variables from IStats interface
    public int health { get { return _health; } set { _health = value; }}
    public int damage { get { return _damage; } set { _health = value; } }

    [SerializeField]
    private int _health;
    [SerializeField]
    private int _damage;

    // Shooting
    float cur_delay;
    float fireRate = 2f;
    Transform target; // Player
    public Transform spawnPoint;

    // Score
    public int inFormationScore;
    public int notInFormationScore;

    //Effects
    public GameObject vfxExplosion;

    public Path pathToFollow;

    //Path Infos
    public int currentWayPointID = 0;
    public float speed = 2;
    public float reachDistance = 0.4f;
    public float rotationSpeed = 5f;

    private float distance; // current distance to next waypoint
    public bool useBezier = false;

    // State Machine
    public enum EnemyStates
    {
        ON_PATH, // is on a path
        FLY_IN, // fly into formation
        IDLE,
        DIVE
    }
    public EnemyStates enemyState;

    public int enemyID;
    public Formation formation;

    ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        target = LoadCharacter.Instance.loadCharObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyStates.ON_PATH:
                MoveOnThePath(pathToFollow);
                break;
            case EnemyStates.FLY_IN:
                MoveToFormation();
                break;
            case EnemyStates.IDLE:
                break;
            case EnemyStates.DIVE:
                MoveOnThePath(pathToFollow);
                // Activate shooting
                SpawnBullet();
                break;
        }
    }

    void MoveToFormation()
    {
        transform.position = Vector3.MoveTowards(transform.position, 
            formation.GetVector(enemyID), speed * Time.deltaTime);

        // rotation of enemy
        var direction = formation.GetVector(enemyID) - transform.position;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            direction = direction.normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        float distanceFormation = Vector3.Distance(transform.position, formation.GetVector(enemyID));
        if (distanceFormation <= 0.0001f)
        {
            transform.SetParent(formation.gameObject.transform);
            transform.eulerAngles = Vector3.zero;

            formation.enemyList.Add(new Formation.EnemyFormation(enemyID, transform.localPosition.x,
                                                                 transform.localPosition.z, this.gameObject));
            enemyState = EnemyStates.IDLE;
        }
    }

    void MoveOnThePath(Path path)
    {
        if (useBezier)
        {
            // moving the enemy
            distance = Vector3.Distance(path.bezierObjList[currentWayPointID], transform.position);
            transform.position = Vector3.MoveTowards(transform.position,
                path.bezierObjList[currentWayPointID], speed * Time.deltaTime);

            // rotation of enemy
            var direction = path.bezierObjList[currentWayPointID] - transform.position;
            if (direction != Vector3.zero)
            {
                direction.y = 0;
                direction = direction.normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            if (distance <= reachDistance)
            {
                currentWayPointID++;
            }

            if (currentWayPointID >= path.bezierObjList.Count)
            {
                currentWayPointID = 0;

                // Diving
                if(enemyState == EnemyStates.DIVE)
                {
                    transform.position = GameObject.Find("SpawnManager").transform.position;
                    Destroy(pathToFollow.gameObject);
                }

                enemyState = EnemyStates.FLY_IN;
            }
        }
        else
        {
            // moving the enemy
            distance = Vector3.Distance(path.pathObjList[currentWayPointID].position, transform.position);
            transform.position = Vector3.MoveTowards(transform.position,
                path.pathObjList[currentWayPointID].position, speed * Time.deltaTime);

            // rotation of enemy
            var direction = path.pathObjList[currentWayPointID].position - transform.position;
            if (direction != Vector3.zero)
            {
                direction.y = 0;
                direction = direction.normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            if (distance <= reachDistance)
            {
                currentWayPointID++;
            }

            if (currentWayPointID >= path.pathObjList.Count)
            {
                currentWayPointID = 0;

                // Diving
                if (enemyState == EnemyStates.DIVE)
                {
                    transform.position = GameObject.Find("SpawnManager").transform.position;
                    Destroy(pathToFollow.gameObject);
                }

                enemyState = EnemyStates.FLY_IN;
            }
        }
    }
    
    public void SpawnSetup(Path path, int ID, Formation form)
    {
        pathToFollow = path;
        enemyID = ID;
        formation = form;
    }

    public void DiveSetup(Path path)
    {
        pathToFollow = path;
        // Parent back to the world
        transform.SetParent(transform.parent.parent);
        enemyState = EnemyStates.DIVE; 
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            // Play sound

            // Get instantiated vfx explosion from object pooler
             objectPooler.SpawnFromPool("VFXExplosion", transform.position, Quaternion.identity);
            // Add score
            if(enemyState == EnemyStates.IDLE)
            {
                GameManager.Instance.AddScore(inFormationScore);
            }
            else
            {
                GameManager.Instance.AddScore(notInFormationScore);
            }

            //// Report to spawn manager
            SpawnManager sp = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            sp.UpdateSpawnedEnemies(this.gameObject);

            gameObject.SetActive(false);
        }
    }

    void SpawnBullet()
    {
        cur_delay += Time.deltaTime;
        if(cur_delay >= fireRate && spawnPoint != null)
        {
            // Spawn enemy bullet from Object Pooler
            spawnPoint.LookAt(target);
            GameObject newBullet = objectPooler.SpawnFromPool("EnemyBullet", spawnPoint.position, spawnPoint.rotation);
            newBullet.GetComponent<Bullet>().SetDamage(damage);
            cur_delay = 0;
        }
    }
}
