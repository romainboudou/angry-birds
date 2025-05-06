using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueueBehaviour : MonoBehaviour
{
    [SerializeField] 
    List<BirdLaunchBehaviour> birds = new List<BirdLaunchBehaviour>();

    public BirdLaunchBehaviour RemoveBird()
    {
        if (IsEmpty())
        {
            return null;
        }

        BirdLaunchBehaviour bird = birds[0];
        birds.RemoveAt(0);
        return bird;
    }

    public bool IsEmpty()
    {
        return birds.Count == 0;
    }
}
