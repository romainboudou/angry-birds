using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdCollisionHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private BirdLaunchBehaviour bird;
    private bool collided = false;

    void Awake()
    {
        InitializeBird();
    }

    private void InitializeBird()
    {
        rb = GetComponent<Rigidbody2D>();
        bird = GetComponent<BirdLaunchBehaviour>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bird"))
        {
            return;
        }

        HandleCollision(collision);

        StartCoroutine(DestroyBirdAfterTime(3f));
    }

    private void HandleCollision(Collision2D collision)
    {
        Debug.Log("Collision détectée avec " + collision.gameObject.name);

        if (!collided)
        {
            HandleFirstCollision();
        }

        float damage = rb.velocity.magnitude;

        ApplyDamageToCollision(collision, damage);

        Debug.Log(damage);
    }

    private void HandleFirstCollision()
    {
        rb.gravityScale = 1;
        rb.velocity = bird.GetVelocity();
        collided = true;
        bird.Collide();
    }

    private void ApplyDamageToCollision(Collision2D collision, float damage)
    {
        DamageBehaviour damageBehaviour = collision.gameObject.GetComponent<DamageBehaviour>();
        if (damageBehaviour != null)
        {
            damageBehaviour.ApplyDamage(damage);
        }
    }

    private IEnumerator DestroyBirdAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay); 
        Destroy(gameObject);
    }
}
