using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PreviewLine : MonoBehaviour
{
    [SerializeField] int segmentAmount;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    Vector2[] lastPositions;
    Vector2[] currentPositions;

    GravityEffector[] effectors;
    PlayerMovement player;
    LineRenderer lineRenderer;
    Rigidbody2D playerRB;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        lineRenderer.positionCount = segmentAmount;
        lastPositions = new Vector2[segmentAmount];
        currentPositions = new Vector2[segmentAmount];
        for (int i = 0; i < segmentAmount; i++) lastPositions[i] = Vector2.zero;
        player = FindObjectOfType<PlayerMovement>();
        playerRB = player.GetComponent<Rigidbody2D>();
        effectors = FindObjectsOfType<GravityEffector>();
    }


    void FixedUpdate()
    {
        int breakIndex;
        CalculatePositions(out breakIndex);
        for(int i = 0; i < breakIndex; i++) lineRenderer.SetPosition(i, currentPositions[i]);
        if(breakIndex < segmentAmount) SetAllPositionsAfter(breakIndex, currentPositions[breakIndex]);
        SetLastPositions();
    }


    void CalculatePositions(out int breakIndex) 
    {
        Vector3 position = player.transform.position;
        Vector3 velocity = playerRB.velocity;

        for (int i = 0; i < segmentAmount; i++)
        {
            position += velocity * Time.fixedDeltaTime;
            for (int j = 0; j < effectors.Length; j++)
            {
                Vector2 gravity = effectors[j].GetGravityAtPoint(position);
                velocity += new Vector3(gravity.x, gravity.y, .0f) * Time.fixedDeltaTime;

                if (Vector3.Distance(effectors[j].transform.position, position) < effectors[j].Radius)
                {
                    currentPositions[i] = position;
                    breakIndex = i;
                    return;
                }
            }
            currentPositions[i] = position;
        }
        breakIndex = segmentAmount;
    }

    void SetLastPositions() 
    {
        for(int i = 0; i < segmentAmount; i++) 
        {
            lastPositions[i] = currentPositions[i];
        }
    }

    void SetAllPositionsAfter(int index, Vector3 position) 
    {
        for (int i = index; i < segmentAmount; i++) lineRenderer.SetPosition(i, position);
    }
}
