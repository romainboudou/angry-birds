using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CODE NON UTILISE
//TOUT EST MAINTENANT GERE DANS SLINGSHOT ET BIRDLAUNCH
public class PreviewBehaviour : MonoBehaviour
{
    public GameObject catPrefab; // Chat à lancer
    public Transform launchPoint; // Point de lancement
    public LineRenderer trajectoryRenderer; // Affichage de la trajectoire

    private Vector2 startPoint; // Position initiale du tir
    private Vector2 endPoint;   // Position finale
    private Rigidbody2D currentCat; // Référence au chat lancé
    private bool isDragging = false;

    public float launchForce = 10f; // Puissance du tir

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Début du clic
        {
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging) // Maintien du clic
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DrawTrajectory();
        }

        if (Input.GetMouseButtonUp(0) && isDragging) // Relâchement
        {
            LaunchCat();
            isDragging = false;
        }
    }

    void DrawTrajectory()
    {
        Vector2 launchVector = startPoint - endPoint;
        Vector2 velocity = launchVector * launchForce;

        int pointsCount = 30;
        Vector3[] trajectoryPoints = new Vector3[pointsCount];

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i * 0.1f;
            trajectoryPoints[i] = (Vector2)launchPoint.position + velocity * t + 0.5f * Physics2D.gravity * t * t;
        }

        trajectoryRenderer.positionCount = pointsCount;
        trajectoryRenderer.SetPositions(trajectoryPoints);
    }

    void LaunchCat()
    {
        GameObject cat = Instantiate(catPrefab, launchPoint.position, Quaternion.identity);
        Rigidbody2D rb = cat.GetComponent<Rigidbody2D>();

        Vector2 launchVector = startPoint - endPoint;
        rb.velocity = launchVector * launchForce;

        trajectoryRenderer.positionCount = 0; // Effacer la trajectoire après le tir
    }

}
