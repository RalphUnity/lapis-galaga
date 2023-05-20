using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    int damage;
    public float speed = 10f;

    public enum Targets
    {
        ENEMY,
        PLAYER
    }

    public Targets target;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void OnTriggerEnter(Collider other)
    {
        if(target == Targets.PLAYER)
        {
            if (other.tag == "Player")
            {
                other.gameObject.GetComponent<Player>().TakeDamage(damage);

                gameObject.SetActive(false);
            }
        }

        if(target == Targets.ENEMY)
        {
            if (other.tag == "Enemy")
            {
                other.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(damage);

                gameObject.SetActive(false);
            }
        }

        if(other.tag == "EndSide")
        {
            gameObject.SetActive(false);
        }
    }
}
