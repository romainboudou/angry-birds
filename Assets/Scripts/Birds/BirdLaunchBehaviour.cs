using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;

public class BirdLaunchBehaviour : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] protected GameObject bird;
    [SerializeField] protected CameraFollow cameraFollow;

    [Header("Initial Conditions")]
    [SerializeField] protected float angle = 45f;       //VARIABLE INITIALE NON UTILISEE
    [SerializeField] protected float distance = 4f;     //VARIABLE INITIALE NON UTILISEE

    [Header("Physics")]
    [SerializeField] protected float m = 0.8f;    // Masse
    [SerializeField] protected float g = 9.81f;   // Gravité
    [SerializeField] protected float k = 10f;     // Raideur du ressort
    [SerializeField] protected float f2 = 0.2f;   // Coeff. frottements

    [Header("VFX")]
    [SerializeField] protected GameObject impactFX;

    [Header("SFX")]
    [SerializeField] protected AudioClip launchSound;
    [SerializeField] protected AudioClip impactSound;
    [SerializeField] protected AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] protected Color lineColor = Color.red;
    [SerializeField] protected float lineDuration = 1f;

    protected bool isLaunched = false;
    protected float initialSpeed = 0f;
    protected float vx = 0;
    protected float vy = 0;

    protected TrailRenderer trail;

    protected virtual void Awake()
    {
        if (bird != null)
            trail = bird.GetComponent<TrailRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (isLaunched)
        {
            ApplyForce();
        }
    }

    protected virtual void Update()
    {
        if (!isLaunched)
        {
            ApplyStretchIdleEffect();
        }
    }

    public void LaunchBird(float alpha, float l1)
    {
        CalculateInitialSpeed(alpha, l1);
        CalculateInitialVelocity(alpha);
        isLaunched = true;

        if (cameraFollow != null)
            cameraFollow.SetTarget(bird.transform);

        StartCoroutine(LaunchSquashEffect());

        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }

        if (audioSource != null && launchSound != null)
            audioSource.PlayOneShot(launchSound);
    }

    IEnumerator LaunchSquashEffect()
    {
        Vector3 originalScale = bird.transform.localScale;
        Vector3 squash = new Vector3(originalScale.x * 0.25f, originalScale.y * 0.4f, 0.3f);
        Vector3 stretch = new Vector3(originalScale.x * 0.25f, originalScale.y * 0.4f, 0.3f);

        // Squash
        bird.transform.localScale = squash;
        yield return new WaitForSeconds(0.05f);

        // Stretch
        bird.transform.localScale = stretch;
        yield return new WaitForSeconds(0.05f);

        // Retour
        bird.transform.localScale = originalScale;
    }

    /// <summary>
    /// Effet d'étirement cartoon lorsque l'utilisateur prépare le tir.
    /// </summary>
    protected void ApplyStretchIdleEffect()
    {
        if (bird == null) return;

        float squashAmount = 0.05f;
        float scaleY = 0.3f + Mathf.Sin(Time.time * 10f) * squashAmount;
        float scaleX = 0.3f - Mathf.Sin(Time.time * 10f) * squashAmount;
        bird.transform.localScale = new Vector3(scaleX, scaleY, 0.3f);
    }

    protected void ApplyForce()
    {
        CalculateNewVelocity();

        Vector3 initialPosition = bird.transform.position;
        bird.transform.position += new Vector3(vx, vy, 0) * Time.deltaTime;
        Vector3 newPosition = bird.transform.position;

        DrawLine(initialPosition, newPosition);
    }

    protected float CalculateInitialSpeed(float alpha, float l1)
    {
        initialSpeed = l1 * Mathf.Sqrt(k / m) * Mathf.Sqrt(1 - Mathf.Pow((m * g * Mathf.Sin(alpha) / (k * l1)), 2));
        return initialSpeed;
    }

    protected void CalculateInitialVelocity(float alpha)
    {
        vx = initialSpeed * Mathf.Cos(alpha);
        vy = initialSpeed * Mathf.Sin(alpha);
    }

    protected void CalculateNewVelocity()
    {
        vx += -f2 * vx * Time.deltaTime;
        vy += -(g + f2 * vy) * Time.deltaTime;
    }

    public Vector3[] GetPreview(float alpha, float l1, int pointsCount, float time = 1.0f)
    {
        Vector3[] trajectoryPoints = new Vector3[pointsCount];

        CalculateInitialSpeed(alpha, l1);
        float lambdaX = initialSpeed * Mathf.Cos(alpha);
        float lambdaY = initialSpeed * Mathf.Sin(alpha) + g / f2;

        for (int i = 0; i < pointsCount; i++)
        {
            float t = time * i / pointsCount;
            float x = (lambdaX / f2) * (1 - Mathf.Exp(-f2 * t));
            float y = (lambdaY / f2) * (1 - Mathf.Exp(-f2 * t)) - g / f2 * t;
            trajectoryPoints[i] = new Vector3(x, y, 0);
        }

        return trajectoryPoints;
    }

    protected void DrawLine(Vector3 begin, Vector3 end)
    {
        Debug.DrawLine(begin, end, lineColor, lineDuration);
    }

    protected float DegToRad(float alpha)
    {
        return alpha * Mathf.Deg2Rad;
    }

    public void Collide()
    {
        isLaunched = false;
        vx = 0;
        vy = 0;

        if (cameraFollow != null)
        {
            cameraFollow.OnImpact(GetVelocity().magnitude);
            cameraFollow.DisableFollow();
        }

        if (audioSource != null && impactSound != null)
            audioSource.PlayOneShot(impactSound);
    }

    public Vector3 GetVelocity()
    {
        return new Vector3(vx, vy, 0);
    }
}

