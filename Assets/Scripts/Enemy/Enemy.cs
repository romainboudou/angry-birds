using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySettings settings;
    [SerializeField] private AudioSource audioSource;

    [Space]
    [Header("Animation Idle")]
    [Tooltip("Amplitude du mouvement idle (position verticale)")]
    [SerializeField] private float idleMoveAmplitude = 0.05f;

    [Tooltip("Vitesse du mouvement idle")]
    [SerializeField] private float idleMoveSpeed = 1.5f;

    [Tooltip("Amplitude du scale idle (effet de respiration)")]
    [SerializeField] private float idleScaleAmplitude = 0.03f;

    [Tooltip("Activer l’animation idle")]
    [SerializeField] private bool enableIdleAnimation = true;

    private Vector3 basePosition;
    private float idleTimer;


    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 initialScale;
    private float currentHealth;
    private bool isDying = false;
    private Coroutine feedbackCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        basePosition = transform.position;
        initialScale = transform.localScale;
        currentHealth = settings.maxHealth;
    }

    private void Update()
    {
        if (enableIdleAnimation && !isDying)
        {
            idleTimer += Time.deltaTime * idleMoveSpeed;

            // Oscillation verticale
            float offsetY = Mathf.Sin(idleTimer) * idleMoveAmplitude;
            transform.position = basePosition + new Vector3(0f, offsetY, 0f);

            // Scale breathing
            float scaleOffset = Mathf.Sin(idleTimer) * idleScaleAmplitude;
            transform.localScale = initialScale + new Vector3(scaleOffset, scaleOffset, 0f);
        }
    }

    void OnEnable()
    {
        ResetState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying) return;

        string tag = collision.gameObject.tag;

        if (tag == "Bird" || tag == "ScreenLimit")
        {
            TakeDamage(currentHealth); // Mort directe
        }
        else if (tag == "Environment")
        {
            float impactForce = collision.relativeVelocity.magnitude;

            if (impactForce >= settings.minImpactForceToDamage)
            {
                float damage = impactForce * settings.impactToDamageRatio;
                TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDying) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            StartCoroutine(EnhancedDeathSequence());
        }
        else
        {
            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = StartCoroutine(DamagePopFeedback());
        }
    }

    /// <summary>
    /// Pop visuel rapide lors de dégâts non létaux
    /// </summary>
    private IEnumerator DamagePopFeedback()
    {
        float duration = 0.1f;
        float timer = 0f;
        Vector3 popScale = initialScale * settings.damagePopScale;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, popScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(popScale, initialScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
    }

    /// <summary>
    /// Mort avec effet complet (FX, flash, slowmo, shake, etc.)
    /// </summary>
    private IEnumerator EnhancedDeathSequence()
    {
        isDying = true;

        // Flash vers blanc puis retour à la couleur d'origine
        Color originalColor = spriteRenderer.color;
        Color flashColor = originalColor;

        float flashTime = 0f;
        while (flashTime < settings.flashDuration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, flashColor, flashTime / settings.flashDuration);
            flashTime += Time.deltaTime;
            yield return null;
        }

        flashTime = 0f;
        while (flashTime < settings.flashDuration)
        {
            spriteRenderer.color = Color.Lerp(flashColor, originalColor, flashTime / settings.flashDuration);
            flashTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;

        // Squash effect (cartoon)
        Vector3 squash = new Vector3(initialScale.x * settings.squashScale, initialScale.y * (2 - settings.squashScale), initialScale.z);
        transform.localScale = squash;
        yield return new WaitForSeconds(0.05f);

        // Reset scale
        transform.localScale = initialScale;

        // Pré-explosion FX
        if (settings.preExplosionFX != null)
            ObjectPool.Instance.Spawn(settings.preExplosionFX, transform.position, Quaternion.identity);

        // Inflation progressive
        float t = 0f;
        Vector3 finalScale = initialScale * settings.growFactor;
        while (t < settings.blinkDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t / settings.blinkDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // Petit slowmotion avant la fin
        if (settings.deathSlowmoTime > 0f)
        {
            Time.timeScale = 0.2f;
            yield return new WaitForSecondsRealtime(settings.deathSlowmoTime);
            Time.timeScale = 1f;
        }

        transform.localScale = finalScale;
        Explode();
    }

    private void Explode()
    {
        if (settings.explosionPrefab != null)
            ObjectPool.Instance.Spawn(settings.explosionPrefab, transform.position, Quaternion.identity);

        if (settings.explosionSound != null)
            AudioSource.PlayClipAtPoint(settings.explosionSound, transform.position);

        CameraShake.Instance?.Shake(settings.screenShakeIntensity);

        gameObject.SetActive(false);
    }

    private void ResetState()
    {
        transform.localScale = initialScale;
        spriteRenderer.color = originalColor;
        spriteRenderer.enabled = true;
        currentHealth = settings.maxHealth;
        isDying = false;
    }
}
