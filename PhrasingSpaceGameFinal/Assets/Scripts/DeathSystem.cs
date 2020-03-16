using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSystem : MonoBehaviour
{


    SpriteRenderer playerRenderer;
    Rigidbody2D playerBody;
    Vector3 respawnLocation;
    List<GameObject> closestPlanets = new List<GameObject>();
    [SerializeField] AudioSource audioSource= null;
    [SerializeField] AudioClip audioClip = null;
    [SerializeField] ParticleSystem explosionSystem = null;
    // Start is called before the first frame update
    void Start()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
        playerBody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D otherCollider)
    {
        
        if (otherCollider.gameObject.tag == "Planet")
        {
            StartCoroutine(Die(otherCollider));
        }
    }

    IEnumerator Die(Collision2D collider)
    {
        explosionSystem.Play();
        audioSource.PlayOneShot(audioClip);
        playerRenderer.enabled = false;
        playerBody.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1.5f);
        if (closestPlanets.Count==0) FindClosestPlanets(0.5f, collider);
        gameObject.transform.position = closestPlanets[1].transform.position + ((closestPlanets[0].transform.position - closestPlanets[1].transform.position)*0.5f);
        playerRenderer.enabled = true;
        closestPlanets.Clear();
    }


    void FindClosestPlanets(float scanRadius, Collision2D otherCollider)
    {
        closestPlanets.Clear();
        RaycastHit2D[] RayclosestPlanets = Physics2D.CircleCastAll(otherCollider.transform.position, scanRadius, new Vector2(1, 1), scanRadius);

        foreach (RaycastHit2D castHit in RayclosestPlanets)
        {

            if (castHit.collider.gameObject.tag == "Planet") {
                if ((castHit.collider.transform.position != otherCollider.transform.position))
                {
                    if (Vector2.Distance(castHit.collider.transform.position,otherCollider.transform.position) > 5 )
                    {
                        closestPlanets.Add(castHit.collider.gameObject);
                    }
                } else
                {
                    closestPlanets.Add(castHit.collider.gameObject);
                }
                

            }
        }
        if (scanRadius > 100)
        {
            Debug.Log("NO PLANETS WITHIN 100 UNITS");
            return;
        }
        if (closestPlanets.Count < 2)
        {
            FindClosestPlanets(scanRadius + 0.5f, otherCollider);
            return;
        }
        
        
        while (closestPlanets.Count > 2)
        {
            closestPlanets.RemoveAt(closestPlanets.Count - 1);
        }
    }

}