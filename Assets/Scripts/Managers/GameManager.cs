using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panel de transition")]
    [SerializeField] private RectTransform transitionPanel;

    [Header("Transition (fermeture)")]
    [SerializeField] private Vector2 startPosition = new Vector2(-3332, 2240);
    [SerializeField] private Vector2 centerPosition = Vector2.zero;
    [SerializeField] private float duration = 0.5f;

    [Header("Raccourci reload")]
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            StartCoroutine(ReloadSceneWithTransition());
        }
    }

    private IEnumerator ReloadSceneWithTransition()
    {
        if (transitionPanel == null) yield break;

        transitionPanel.anchoredPosition = startPosition;

        // Entrée : haut gauche -> centre
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);
            transitionPanel.anchoredPosition = Vector2.Lerp(startPosition, centerPosition, progress);
            yield return null;
        }

        transitionPanel.anchoredPosition = centerPosition;

        yield return new WaitForSeconds(0.05f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
