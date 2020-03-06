﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameSettingsSO settings;
    [SerializeField] UnityEvent onStartBoost = new UnityEvent();
    [SerializeField] UnityEvent onStopBoost = new UnityEvent();
    Rigidbody2D rb;
    bool boosting = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        //rotate with mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Vector3 lookAt = mousePos - transform.position;
        float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(.0f, .0f, angle-90.0f);



        //boost if clicked
        if (Input.GetKey(settings.boostKey)) 
        {
            if(boosting == false) 
            {
                boosting = true;
                onStartBoost.Invoke();
            }
            Vector3 velocity = lookAt * settings.boostSpeed * Time.deltaTime;
            rb.velocity += new Vector2(velocity.x, velocity.y);
        } else 
        {
            if(boosting == true) 
            {
                boosting = false;
                onStopBoost.Invoke();
            }
        }
    }
}
