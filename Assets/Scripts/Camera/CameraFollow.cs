using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Suivi de l'objet")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Zoom dynamique")]
    [SerializeField] private float defaultZoom = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxImpact = 20f;
    [SerializeField] private float zoomResetSpeed = 2f;

    [Header("Tremblement")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 0.5f;

    [Header("Post-Processing")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float vignetteDefault = 0.2f;
    [SerializeField] private float vignetteImpact = 0.26f;

    private Transform target;
    private Vector3 initialPosition;
    private Camera cam;
    private Vignette vignette;
    private bool followEnabled;
    private bool returnToStart;
    private bool isShaking;
    private bool isZooming;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        initialPosition = transform.position;
        cam.orthographicSize = defaultZoom;

        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out Vignette v))
            vignette = v;

        if (vignette != null)
            vignette.intensity.value = vignetteDefault;
    }

    private void LateUpdate()
    {
        if (followEnabled && target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.z = offset.z;

            Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothed;
        }
        else if (returnToStart)
        {
            Vector3 smoothed = Vector3.Lerp(transform.position, initialPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothed;

            if (Vector3.Distance(transform.position, initialPosition) < 0.05f)
            {
                transform.position = initialPosition;
                returnToStart = false;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        followEnabled = true;
        returnToStart = false;
    }

    public void DisableFollow()
    {
        followEnabled = false;
        returnToStart = true;
    }

    public void OnImpact(float impactForce)
    {
        DisableFollow();
        float t = Mathf.Clamp01(impactForce / maxImpact);
        float targetZoom = Mathf.Lerp(defaultZoom, minZoom, t);
        StartCoroutine(ZoomShakeSequence(targetZoom));
    }

    private IEnumerator ZoomShakeSequence(float targetZoom)
    {
        isZooming = true;

        float originalZoom = cam.orthographicSize;
        float timer = 0f;
        float zoomInDuration = 0.05f;

        // Phase 1 - Zoom IN + vignette up
        while (timer < zoomInDuration)
        {
            cam.orthographicSize = Mathf.Lerp(originalZoom, targetZoom, timer / zoomInDuration);
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(vignetteDefault, vignetteImpact, timer / zoomInDuration);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.orthographicSize = targetZoom;
        if (vignette != null) vignette.intensity.value = vignetteImpact;

        // Phase 2 - Shake
        yield return StartCoroutine(Shake(shakeDuration, shakeIntensity));

        // Phase 3 - Zoom OUT + vignette reset
        while (Mathf.Abs(cam.orthographicSize - defaultZoom) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultZoom, Time.deltaTime * zoomResetSpeed);

            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, vignetteDefault, Time.deltaTime * 4f);

            yield return null;
        }

        cam.orthographicSize = defaultZoom;
        if (vignette != null) vignette.intensity.value = vignetteDefault;

        isZooming = false;
    }

    private IEnumerator Shake(float duration, float intensity)
    {
        isShaking = true;
        float elapsed = 0f;
        Vector3 basePosition = transform.position;

        while (elapsed < duration)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * intensity;
            transform.position = basePosition + new Vector3(shakeOffset.x, shakeOffset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = basePosition;
        isShaking = false;
    }
}
