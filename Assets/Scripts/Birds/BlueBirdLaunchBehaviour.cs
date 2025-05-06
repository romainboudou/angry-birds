using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBirdLaunchBehaviour : BirdLaunchBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float angleOffset = 15f;

    private bool canSplit = true;

    protected override void Update()
    {
        base.Update();
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.Space) && isLaunched && canSplit)
        {
            Split();
        }
    }

    private void Split()
    {
        SpawnChildrens();
    }

    private void SpawnChildrens()
    {
        angleOffset *= Mathf.Deg2Rad;
        GameObject child1 = Instantiate(gameObject, transform.position, Quaternion.identity);
        GameObject child2 = Instantiate(gameObject, transform.position, Quaternion.identity);

        BlueBirdLaunchBehaviour bird1 = child1.GetComponent<BlueBirdLaunchBehaviour>();
        BlueBirdLaunchBehaviour bird2 = child2.GetComponent<BlueBirdLaunchBehaviour>();

        bird1.InitializeChild(angleOffset, vx, vy);
        bird2.InitializeChild(-angleOffset, vx, vy);

        SetSplit(false);
    }

    private void InitializeChild(float angleOffset, float initialvx, float initialvy)
    {
        this.vx = initialvx;
        this.vy = initialvy;

        AdjustRotation(angleOffset);
        SetSplit(false);
        isLaunched = true;
    }

    public void AdjustRotation(float angleOffset)
    {
        float newvx = Mathf.Cos(angleOffset) * vx + Mathf.Sin(angleOffset) * vy;
        float newvy = -Mathf.Sin(angleOffset) * vx + Mathf.Cos(angleOffset) * vy;

        vx = newvx;
        vy = newvy;
    }

    public void SetSplit(bool split) 
    { 
        canSplit = false;
    }
}
