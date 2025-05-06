using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBirdLaunchBehaviour : BirdLaunchBehaviour
{
    //Ancien code où l'oiseau suit l'endroit où on clique

    //[Header("Parameters")]
    //[SerializeField] private float strength = 10f;
    //[SerializeField] private Transform TargetPoint;

    //private bool canDash = true;

    //protected override void Update()
    //{
    //    base.Update();
    //    if (Input.GetKey(KeyCode.Space) && isLaunched && canDash)
    //    {
    //        Dash();
    //    }
    //}
    //private void Dash()
    //{
    //    Vector3 direction = TargetPoint.position - transform.position;
    //    angle = Mathf.Atan2(direction.y, direction.x);
    //    LaunchBird(angle, strength);
    //    canDash = false;
    //}

    [Header("Parameters")]
    [SerializeField] private float dashStrength = 15f;

    private bool canDash = true;

    protected override void Update()
    {
        base.Update();
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.Space) && isLaunched && canDash)
        {
            Dash();
        }
    }

    private void Dash()
    {
        Vector2 direction = new Vector2(vx, vy);
        angle = Mathf.Atan2(direction.y, direction.x);
        LaunchBird(angle, dashStrength);
        canDash = false;
    }
}
