using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathSystem : MonoBehaviour
{
    SpriteRenderer playerRenderer;
    Rigidbody2D playerBody;
    Vector3 respawnLocation;
    List<GameObject> closestPlanets = new List<GameObject>();
    Collider2D playerCollider;

    [HideInInspector] public UnityEngine.Events.UnityEvent onDie = new UnityEngine.Events.UnityEvent();
    [HideInInspector] public UnityEngine.Events.UnityEvent onRevive = new UnityEngine.Events.UnityEvent();

    [SerializeField] AudioSource audioSource= null;
    [SerializeField] AudioClip audioClip = null;
    [SerializeField] ParticleSystem explosionSystem = null;
    [SerializeField] Image fuelBar = null;
    [SerializeField] GameObject boostSystem = null;
    // Start is called before the first frame update
    void Start()
    {
        var player = FindObjectOfType<PlayerMovement>().gameObject;
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerBody = gameObject.GetComponent<Rigidbody2D>();
        playerCollider = gameObject.GetComponent<Collider2D>();

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
        onDie.Invoke();
        fuelBar.enabled = false;
        explosionSystem.Play();
        audioSource.PlayOneShot(audioClip);
        playerBody.simulated = false;
        playerRenderer.enabled = false;
        playerCollider.enabled = false;
        boostSystem.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        if (closestPlanets.Count==0) FindClosestPlanets(0.5f, collider);
        gameObject.transform.position = closestPlanets[1].transform.position + ((closestPlanets[0].transform.position - closestPlanets[1].transform.position)*0.5f);
        playerCollider.enabled = true;
        playerRenderer.enabled = true;
        playerBody.simulated = true;
        boostSystem.SetActive(true);
        playerBody.velocity = new Vector2(0,0);
        closestPlanets.Clear();
        fuelBar.enabled = true;
        onRevive.Invoke();
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