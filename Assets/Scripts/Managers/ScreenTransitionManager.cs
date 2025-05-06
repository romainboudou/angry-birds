using System.Collections;
using UnityEngine;

public class SceneOpenTransition : MonoBehaviour
{
    [Header("Panel à animer")]
    [SerializeField] private RectTransform transitionPanel;

    [Header("Positions")]
    [SerializeField] private Vector2 centerPosition = Vector2.zero;
    [SerializeField] private Vector2 exitPosition = new Vector2(3332, -2240);

    [Header("Durée")]
    [SerializeField] private float duration = 0.5f;

    private void Start()
    {
        if (transitionPanel != null)
        {
            transitionPanel.anchoredPosition = centerPosition;
            StartCoroutine(MovePanel(transitionPanel, centerPosition, exitPosition));
        }
    }

    private IEnumerator MovePanel(RectTransform panel, Vector2 from, Vector2 to)
    {
        float t = 0f;
        panel.anchoredPosition = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);
            panel.anchoredPosition = Vector2.Lerp(from, to, progress);
            yield return null;
        }

        panel.anchoredPosition = to;
    }
}
