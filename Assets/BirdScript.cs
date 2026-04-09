using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public float flapStrength;
    public LogicScript logic;
    public bool birdIsAlive = true;
    public AudioSource flapSound;
    public AudioSource hitSound;
    public AudioSource fallSound;
    public float rotationSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) == true && birdIsAlive==true) || (Input.GetMouseButtonDown(0) == true && birdIsAlive == true))
        {
            myRigidBody.linearVelocity = Vector2.up * flapStrength;
            flapSound.Play();
        }

        if (myRigidBody.linearVelocity.y > 0)
        {
            // Point the nose up immediately (35 degrees on the Z axis)
            transform.rotation = Quaternion.Euler(0, 0, 35f);
        }
        // If the bird is falling DOWN
        else
        {
            // Create a target rotation pointing straight down (-90 degrees)
            Quaternion targetRotation = Quaternion.Euler(0, 0, -70f);

            // Smoothly tilt from our current rotation towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (transform.position.y < -20 || transform.position.y > 50)
        {
            if (birdIsAlive)
            {
                

                fallSound.Play();
                logic.gameOver();
                birdIsAlive = false;
            }
        }
       
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {   
        if(birdIsAlive)
        {
            hitSound.Play();
            logic.gameOver();
            birdIsAlive = false;
        }
        
        
    }
}
