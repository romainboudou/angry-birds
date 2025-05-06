using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBirdLaunchBehaviour : BirdLaunchBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float swingStrength = 2.0f;
    [SerializeField] private float swingDuration = 1.0f;

    private bool canswing = true;
    private bool isSwinging = false;
    private float swingDirection;
    private float swingTimer;

    protected override void Update()
    {
        base.Update();
        CheckInput();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isLaunched && isSwinging)
        {
            ApplySwingForce();
        }
    }

    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.Space) && isLaunched && canswing)
        {
            Swing();
        }
    }

    private void Swing()
    {
        isSwinging = true;
        swingTimer = swingDuration;
        canswing = false;
        swingDirection = -Mathf.Sign(vx);
    }

    private void ApplySwingForce()
    {
        vx += swingDirection * swingStrength * Time.deltaTime;

        swingTimer -= Time.deltaTime;

        if (swingTimer <= 0f)
        {
            isSwinging = false;
        }
    }
}
