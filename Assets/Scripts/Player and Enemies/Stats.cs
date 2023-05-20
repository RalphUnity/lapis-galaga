using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStats 
{
    int health { get; set; }
    int damage { get; set; }

    void TakeDamage(int amount);
}
