using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")] 
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private NewPlayerMovement pm;

    [Header("Sliding")] 
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    public float startYScale;

    [Header("Input")] 
    public KeyCode slideKey = KeyCode.LeftControl;

    private int horizontalInput;
    private int verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<NewPlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void FixedUpdate()
    {
        if(pm.sliding)
            SlidingMovement();
        
    }

    private void Update()
    {
        horizontalInput = (int)Input.GetAxisRaw("Horizontal");
        verticalInput = (int)Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey))
        {
            StartSlide();
        }
        if(Input.GetKeyUp(slideKey) && pm.sliding)
            StopSlide();
    }

    private void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f,ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        if (verticalInput == 0 && horizontalInput == 0)
            verticalInput = 1;
        
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        //sliding normal
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        //sliding down a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }
        
        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        pm.sliding = false;
        
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
