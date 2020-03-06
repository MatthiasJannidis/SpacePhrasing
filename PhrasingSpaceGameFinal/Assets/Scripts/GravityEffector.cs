using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEffector : MonoBehaviour
{
    [SerializeField] float weight;
    [SerializeField] float radius;
    public float Radius { get { return radius; } }
    Rigidbody2D playerRb;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody2D>();
    }

    public Vector2 GetGravityAtPoint(Vector2 point) 
    {
        Vector3 dir = point - new Vector2(transform.position.x, transform.position.y);
        float distance = Mathf.Max(dir.magnitude - radius, .0f);
        dir.Normalize();
        return -dir * (1.0f / (radius + distance * distance)) * weight;
    }

    // Update is called once per frame
    void Update()
    {
        playerRb.velocity += GetGravityAtPoint(playerRb.transform.position)*Time.deltaTime;
    }
}
