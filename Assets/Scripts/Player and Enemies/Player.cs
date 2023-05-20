using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IStats
{
    public static Player Instance;

    //Bullet
    public Transform[] bulletSpawns;
    public float fireRate = 0.5f;

    ObjectPooler objectPooler;
    Joystick joystick;

    //Health and Damage
    public int health { get { return _health; } set { _health = value; } }
    public int damage { get { return _damage; } set { _health = value; } }

    [SerializeField]
    private int _health;
    [SerializeField]
    private int _damage;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set player health
        GameManager.Instance.health = health;
        UIScript.Instance.UpdateHealthText(health);
        objectPooler = ObjectPooler.Instance;
        joystick = Joystick.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Joystick Movement
        transform.Translate(Vector3.left * Time.deltaTime * -joystick.Horizontal * 12f);

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.1f, 0.9f);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    public void Fire()
    {
        // Spawn bullet from Object Pooler
        GameObject newBullet = objectPooler.SpawnFromPool("Bullet",
                                                         bulletSpawns[0].position,
                                                         bulletSpawns[0].rotation);
        // Add damage to Bullet
        newBullet.GetComponent<Bullet>().SetDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        GameManager.Instance.UpdateLife(amount);
        if (health <= 0)
        {

            // Show particle effect
            objectPooler.SpawnFromPool("VFXExplosion", transform.position, Quaternion.identity);

            // Deactivate the player
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(health);
        }
    }
}
