using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Settings", fileName = "NewEnemySettings")]
public class EnemySettings : ScriptableObject
{
    [Header("Santé & Dégâts")]
    [Tooltip("Points de vie maximum de l'ennemi.")]
    public float maxHealth = 100f;

    [Tooltip("Multiplicateur de dégâts basé sur la force de l'impact.")]
    public float impactToDamageRatio = 10f;

    [Header("Seuil d'impact minimum")]
    [Tooltip("Force minimale nécessaire pour que l'objet de l'environnement inflige des dégâts.")]
    public float minImpactForceToDamage = 2f;


    [Header("Explosion finale")]
    [Tooltip("Prefab de l'effet d'explosion lors de la destruction.")]
    public GameObject explosionPrefab;

    [Tooltip("Son joué à l'explosion.")]
    public AudioClip explosionSound;

    [Tooltip("Intensité du screen shake lors de l'explosion.")]
    public float screenShakeIntensity = 0.5f;


    [Header("Feedback pré-explosion")]
    [Tooltip("FX déclenché avant l'explosion (fumée, étincelles, etc.).")]
    public GameObject preExplosionFX;

    [Tooltip("Durée de l'inflation finale avant l'explosion.")]
    public float blinkDuration = 0.5f;

    [Tooltip("Facteur de grossissement final avant l'explosion.")]
    public float growFactor = 1.5f;

    [Tooltip("Appliquer un squash visuel avant la mort.")]
    public float squashScale = 0.9f;

    [Tooltip("Ralenti temporel rapide juste avant l'explosion (en secondes temps réel).")]
    public float deathSlowmoTime = 0.05f;

    [Tooltip("Durée du flash visuel à la mort.")]
    public float flashDuration = 0.05f;

    [Header("Feedback de dégâts non létaux")]
    [Tooltip("Facteur de pop scale temporaire lors d’un coup non létal.")]
    public float damagePopScale = 1.15f;

    [Header("Debug")]
    [Tooltip("Afficher les valeurs debug dans la console.")]
    public bool debugLogs = false;
}
