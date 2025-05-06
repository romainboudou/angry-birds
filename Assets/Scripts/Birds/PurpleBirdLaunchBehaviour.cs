using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleBirdLaunchBehaviour : BirdLaunchBehaviour
{
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        float deltaX = mousePos.x - transform.position.x;
        float deltaY = mousePos.y - transform.position.y;
        angle = Mathf.Atan(deltaY / deltaX);

        if (deltaX < 0)
        {
            angle += Mathf.PI;
        }
        else if (deltaY < 0)
        {
            angle += 2 * Mathf.PI;
        }
        
        LaunchBird(angle, dashStrength);
        canDash = false;
    }
}
