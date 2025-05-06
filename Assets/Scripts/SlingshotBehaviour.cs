using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotBehaviour : MonoBehaviour
{
    [SerializeField] QueueBehaviour queue;      //Liste des oiseaux
    [SerializeField] Transform birdSeat;        //Là où l'oiseau apparait
    [SerializeField] float maxDistance = 8f;
    [SerializeField] float minDistance = 1f;
    [SerializeField] int pointsCount = 10;

    public LineRenderer trajectoryRenderer; // Affichage de la trajectoire

    private BirdLaunchBehaviour currentBird;    //Oiseau actuellement utilisé

    [Header("Shooting")]
    private Vector2 startPoint; // Position initiale du tir
    private Vector2 endPoint;   // Position finale
    private bool isDragging = false;
    private float distance;
    private float angle;

    [SerializeField] private float timeBetweenBirds = 2f;

    private void Start()
    {
        GetNewBird();
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0)) // Début du clic
        {
            startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging) // Maintien du clic
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateDistanceAndAngle(startPoint, endPoint);
            if (CheckDistance())
            {
                DrawTrajectory();
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging && CheckDistance()) // Relâchement
        {
            currentBird.LaunchBird(angle,distance);
            isDragging = false;
            StartCoroutine(WaitAndGetNewBird());
        }
    }

    //ATTENTION CODE DE ROMAIN
    private void DrawTrajectory()
    {
        Vector3[] trajectoryPoints = currentBird.GetPreview(angle, distance, pointsCount);

        trajectoryRenderer.positionCount = pointsCount;
        trajectoryRenderer.SetPositions(trajectoryPoints);
    }
    //FIN DU CODE DE ROMAIN

    private bool CheckDistance()
    {
        if (distance < minDistance)
        {
            return false;
        }
        return true;
    }
    private void CalculateDistanceAndAngle(Vector3 start, Vector3 end)
    {
        Vector3 direction = start - end;
        distance = direction.magnitude;
        if (distance > maxDistance)
        {
            distance = maxDistance;
        }
        angle = Mathf.Atan2(direction.y, direction.x);
    }

    private void GetNewBird()
    {
        currentBird = queue.RemoveBird();

        if (currentBird != null)
        {
            currentBird.transform.position = birdSeat.position;
        }
    }
    private IEnumerator WaitAndGetNewBird()
    {
        yield return new WaitForSeconds(timeBetweenBirds);
        GetNewBird();
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
        guiStyle.fontSize = 30;
        guiStyle.normal.textColor = Color.white;

        Rect distanceRect = new Rect(10, 10, 600, 40);
        Rect angleRect = new Rect(10, 60, 600, 40);

        GUI.Label(distanceRect, "Distance: " + distance.ToString("F2"), guiStyle);
        GUI.Label(angleRect, "Angle: " + (angle * Mathf.Rad2Deg).ToString("F2") + "°", guiStyle);
    }
}
