using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    [Header("Damage Parameters")]
    [SerializeField] private float health = 10f;
    [SerializeField] private float damageMultiplier = 1f;

    public void ApplyDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} est détruit !");
        }
    }
}
