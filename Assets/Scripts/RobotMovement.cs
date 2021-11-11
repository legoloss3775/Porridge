using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public  float           walkSpeed;
    private Rigidbody2D     playerRb;
    private BoxCollider2D   playerCol;

    public void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        playerRb.velocity = GetMovementDirection() * walkSpeed;
    }
    private Vector2 GetMovementDirection()
    {
        Vector2 direction = Vector2.zero;

        direction.x += Input.GetAxis("Horizontal");
        direction.y += Input.GetAxis("Vertical");
        return direction;
    }
}
