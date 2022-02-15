using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // First Person Player Logic
    // TODO: FIX VELOCITY ISSUE
    public CharacterController controller;

    private float speed = 12f;
    private float gravity = -9.81f;

    public Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    private bool isGrounded;

    // Riemann Surface Player Logic
    public int riemannLevel;
    
    /// <summary>
    /// Called every tick
    /// Gets the position of the player on the Riemann Surface. 
    /// For the player's XZ position in the world, we can treat Z as the real coordinate and X as the imaginary component
    /// For the player's height in the world, want the real part of z^1/n where n is predecided -> not sure if particularly useful for us
    /// To map the player uniquely in the world, want the imginary part of z^1/n. 
    /// Given number of rotations around pole, x, and z, output the current color block/level = round down color block.
    /// </summary>
    void GetRiemannLevel()
    {
        
    }

    /// <summary>
    /// Once player rotates around the pivot point of a room, increment level. 
    /// </summary>
    void AscendLevel()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Check for layers in the list of groundMask
        this.isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (this.isGrounded && this.velocity.y < 0)
        {
            Debug.Log("Is grounded");
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Movement is all local to the player's facing direction.
        Vector3 move = transform.right * x + transform.forward * z;

        // make it framerate independent
        this.controller.Move(move * speed * Time.deltaTime);

        this.velocity.y += this.gravity * Time.deltaTime;

        // given that deltaY = 1/2 * gravity * t^2 = 1/2 v * t
        controller.Move(velocity * Time.deltaTime);
    }
}
