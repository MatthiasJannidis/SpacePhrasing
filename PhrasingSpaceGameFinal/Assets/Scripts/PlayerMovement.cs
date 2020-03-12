using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameSettingsSO settings;
    [SerializeField] UnityEvent onStartBoost = new UnityEvent();
    [SerializeField] UnityEvent onStopBoost = new UnityEvent();
    [SerializeField] UnityEngine.UI.Image boostBar;
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] AudioClip boostClip = null;
    Rigidbody2D rb;
    bool boosting = false;
    Timer boostRegenTimer = new Timer(.0f);
    float boostFuel = .0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boostFuel = settings.boostFuelAmount;
    }

    void FixedUpdate()
    {
        //rotate with mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Vector3 lookAt = mousePos - transform.position;
        float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(.0f, .0f, angle-90.0f);

        //boost if clicked and enough boost
        if (Input.GetKey(settings.boostKey) && boostFuel > .001f) 
        {
            if (audioSource.isPlaying == false) audioSource.PlayOneShot(boostClip);
            if(boosting == false)
            {
                boosting = true;
                onStartBoost.Invoke();
            }
            Vector3 velocity = lookAt * settings.boostSpeed * Time.deltaTime;
            rb.velocity += new Vector2(velocity.x, velocity.y);
            IncrementFuel(-Time.deltaTime * settings.boostFuelBurnSpeed);
        } else 
        {
            if(boosting == true) 
            {
                if (audioSource.isPlaying == true) audioSource.Stop();
                boosting = false;
                boostRegenTimer.reset(settings.boostRegenCooldown);
                onStopBoost.Invoke();
            
            }
            boostRegenTimer.tick();

            if (boostRegenTimer.isDone) 
            {
                IncrementFuel(Time.deltaTime * settings.boostRegenSpeed);
            }
        }
    }

    void IncrementFuel(float amount)
    {
        boostFuel += amount; 
        boostFuel = Mathf.Clamp(boostFuel, .0f, settings.boostFuelAmount);
        boostBar.fillAmount = boostFuel / settings.boostFuelAmount;
    }
}
