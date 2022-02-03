using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Crouching, Walking, Sprinting, Falling};

public class PlayerMovement : MonoBehaviour
{
    public MoveType moveType = MoveType.Walking;
    MoveType lastMoveType;

    public float walkSpeed = 2f;
    public float sprintSpeed = 3.5f;
    public float crouchSpeed = 1.25f;
    public float fallMoveSpeed = 0.5f;

    float moveSpeed = 0;

    public float crouchHeight = 0.5f;
    public float jumpHeight = 3f;

    public float gravity = 29.43f;

    public CharacterController controller;

    Vector3 velocity;
    Vector3 momentumVelocity;

    public Transform groundCheck;
    public float groundCheckDist = 0.4f;
    public LayerMask walkableMask;

    bool isGrounded;

    private void Start()
    {
        moveSpeed = walkSpeed;
        moveType = MoveType.Walking;
        lastMoveType = MoveType.Walking;
    }

    private void changeMovementType(MoveType to)
    {
        if(to == MoveType.Crouching && moveType != MoveType.Crouching)
        {

            moveType = MoveType.Crouching;
            moveSpeed = crouchSpeed;
            transform.localScale = new Vector3 (1f, crouchHeight, 1f);
            transform.localPosition -= new Vector3(0f, 0.4f, 0f);


        }else if (to == MoveType.Walking || moveType == MoveType.Crouching && to == MoveType.Crouching)
        {

            moveType = MoveType.Walking;
            moveSpeed = walkSpeed;
            transform.localScale = new Vector3(1f, 1f, 1f);

        }
        else if (to == MoveType.Sprinting)
        {

            moveType = MoveType.Sprinting;
            moveSpeed = sprintSpeed;
            transform.localScale = new Vector3(1f, 1f, 1f);

        }else if (to == MoveType.Falling)
        {
            if(moveType != MoveType.Falling)
                lastMoveType = moveType;
            moveType = MoveType.Falling;
            moveSpeed = fallMoveSpeed;

        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDist, walkableMask);

        //select type of movement
        if (!isGrounded)
        {
            changeMovementType(MoveType.Falling);
        }
        if(isGrounded && moveType == MoveType.Falling)
        {
            changeMovementType(lastMoveType);
        }
        else
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                changeMovementType(MoveType.Sprinting);
            }
            else if (moveType == MoveType.Sprinting && !Input.GetKey(KeyCode.LeftShift))
            {
                changeMovementType(MoveType.Walking);
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                changeMovementType(MoveType.Crouching);
            }
        }

        //move

        if (isGrounded  && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float yVel = velocity.y;

        if (isGrounded)
        {
            velocity = (transform.right * x + transform.forward * z) * moveSpeed;
            velocity.y = yVel;
            controller.Move(new Vector3(velocity.x, 0f, velocity.z) * Time.deltaTime);
            momentumVelocity = velocity;
            momentumVelocity.y = 0f;
        }
        else
        {
            velocity = (transform.right * x + transform.forward * z) * moveSpeed;
            velocity.y = yVel;

            controller.Move((momentumVelocity + new Vector3(velocity.x, 0f, velocity.z)) * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2 * gravity);
        }

        velocity.y -= gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }

}
