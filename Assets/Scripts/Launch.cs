using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

enum LaunchType
{
    SansFrottement,
    AvecFrottement,
    SansFrottementRecurrence,
    AvecFrottementRecurrence
}

//CODE NON UTILISE
//LISTE DE FORMULES
public class Launch : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private GameObject bird;
    [SerializeField] private bool animate;
    [SerializeField] private float animationTime;
    [SerializeField] private Color lineColor;
    [SerializeField] private float lineDuration = 1f;
    [SerializeField] private LaunchType launchType = LaunchType.SansFrottement;
    
    [Header("Initial Conditions")]
    [SerializeField] private float angle = 45f;
    [SerializeField] private float distance = 4f;
    
    [Header("Physics")]
    [SerializeField] private float m = 0.8f;
    [SerializeField] private float g = 9.81f;
    [SerializeField] private float k = 10f;
    [SerializeField] private float f2 = 0.2f;

    private void Update()
    {
        if (animate)
        {
            animate = false;
            float alpha = angle * Mathf.Deg2Rad;
            AfficherOiseau(alpha, distance);
        }
    }
    
    private float VitesseInitiale(float alpha, float l1)
    {
        float vEject = l1 * Mathf.Sqrt(k / m) * Mathf.Sqrt(1 - Mathf.Pow((m * g * Mathf.Sin(alpha) / (k * l1)), 2));
        return vEject;
    }

    private (List<float>, List<float>) LancerOiseau(float alpha, float l1)
    {
        float v0 = VitesseInitiale(alpha, l1);
        float tMax = 2 * v0 * Mathf.Sin(alpha) / g;
        int capacity = 100;

        List<float> listeT = new List<float>(capacity);
        List<float> x = new List<float>(capacity);
        List<float> y = new List<float>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            float t = tMax * i / (capacity - 1);
            listeT.Add(t);
            x.Add(v0 * Mathf.Cos(alpha) * t);
            y.Add(v0 * Mathf.Sin(alpha) * t - 0.5f * g * t * t);
        }
        return (x, y);
    }
    
    private (List<float>, List<float>) LancerOiseauFrottement(float alpha, float l1)
    {
        float v0 = VitesseInitiale(alpha, l1);
        float tMax = 2 * v0 * Mathf.Sin(alpha) / g;
        float lambdaX = v0 * Mathf.Cos(alpha);
        float lambdaY = v0 * Mathf.Sin(alpha) + g / f2;

        List<float> listeT = new List<float>(100);
        List<float> x = new List<float>(100);
        List<float> y = new List<float>(100);

        for (int i = 0; i < 100; i++)
        {
            float t = tMax * i / 99;
            listeT.Add(t);
            x.Add((lambdaX / f2) * (1 - Mathf.Exp(-f2 * t)));
            y.Add((lambdaY / f2) * (1 - Mathf.Exp(-f2 * t)) - g / f2 * t);
        }

        return (x, y);
    }
    
    private (List<float>, List<float>) LancerOiseauRecurrence(float alpha, float l1)
    {
        float v0 = VitesseInitiale(alpha, l1);
        float dt = 0.01f;
        float x = 0, y = 0;
        List<float> listeX = new List<float> { 0 };
        List<float> listeY = new List<float> { 0 };
        float vx = v0 * Mathf.Cos(alpha);
        float vy = v0 * Mathf.Sin(alpha);

        while (listeY[listeY.Count - 1] >= 0)
        {
            x += vx * dt;
            y += vy * dt;
            listeX.Add(x);
            listeY.Add(y);
            vy += -g * dt;
        }

        return (listeX, listeY);
    }
    
    private (List<float>, List<float>) LancerOiseauFrottementRecurrence(float alpha, float l1)
    {
        float v0 = VitesseInitiale(alpha, l1);
        float dt = 0.01f;
        float x = 0, y = 0;
        List<float> listX = new List<float> { 0 };
        List<float> listY = new List<float> { 0 };
        float vx = v0 * Mathf.Cos(alpha);
        float vy = v0 * Mathf.Sin(alpha);

        while (listY[listY.Count - 1] >= 0)
        {
            x += vx * dt;
            y += vy * dt;
            listX.Add(x);
            listY.Add(y);
            vx += -f2 * vx * dt;
            vy += - (g + f2 * vy) * dt;
        }

        return (listX, listY);
    }
    
    private IEnumerator BirdCoroutine(float alpha, float l1)
    {
        (List<float> x, List<float> y) = LancerOiseauFrottementRecurrence(alpha, l1);

        switch (launchType)
        {
            case LaunchType.SansFrottement:
                (x, y) = LancerOiseau(alpha, l1);
                break;
            case LaunchType.AvecFrottement:
                (x, y) = LancerOiseauFrottement(alpha, l1);
                break;
            case LaunchType.SansFrottementRecurrence:
                (x, y) = LancerOiseauRecurrence(alpha, l1);
                break;
            case LaunchType.AvecFrottementRecurrence:
                (x, y) = LancerOiseauFrottementRecurrence(alpha, l1);
                break;
        }
    
        Vector3 startPos = transform.position;
    
        float timePerSegment = animationTime / (x.Count - 1);
    
        for (int i = 0; i < x.Count - 1; i++)
        {
            Vector3 currentPos = new Vector3(x[i], y[i], 0) + startPos;
            Vector3 nextPos = new Vector3(x[i + 1], y[i + 1], 0) + startPos;
        
            float elapsedTime = 0f;
        
            while (elapsedTime < timePerSegment)
            {
                float t = elapsedTime / timePerSegment;
                bird.transform.position = Vector3.Lerp(currentPos, nextPos, t);
            
                // Draw trajectory line
                Debug.DrawLine(currentPos, nextPos, lineColor, lineDuration);
            
                elapsedTime += Time.deltaTime;
                
                yield return new WaitForSeconds(Time.deltaTime);
            }
        
            bird.transform.position = nextPos;
        }
        
        lineColor = Random.ColorHSV(0, 1, 0, 1, 0, 1);
    }

    private void AfficherOiseau(float alpha, float l1)
    {
        StartCoroutine(BirdCoroutine(alpha, l1));
    }
}
