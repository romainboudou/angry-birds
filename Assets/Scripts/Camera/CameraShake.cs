using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Shake(float intensity)
    {
        // À implémenter selon ton système de caméra (Cinemachine ou perso)
        Debug.Log($"Camera shake with intensity {intensity}");
    }
}
