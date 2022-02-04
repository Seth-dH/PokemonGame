using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Crouching, Walking, Sprinting};

public class PlayerMovement : MonoBehaviour
{
    public MoveType moveType = MoveType.Walking;

    public float walkSpeed = 2f;
    public float sprintSpeed = 3.5f;
    public float crouchSpeed = 1.25f;

    float moveSpeed = 0;

    public float crouchHeight = 0.5f;
    public float jumpHeight = 3f;

    public float gravity = 29.43f;

    public CharacterController controller;

    Vector3 velocity;

    public Transform groundCheck;
    public float groundCheckDist = 0.4f;
    public LayerMask walkableMask;

    bool isGrounded;

    private void Start()
    {
        moveSpeed = walkSpeed;
        moveType = MoveType.Walking;
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

        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDist, walkableMask);

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

        if (isGrounded )
        {
            controller.stepOffset = 0.3f;
            if(velocity.y < 0f)
                velocity.y = -2f;
        }
        else
        {
            controller.stepOffset = 0.0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;


        controller.Move(move * Time.deltaTime * moveSpeed);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2 * gravity);
        }

        velocity.y -= gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }

}
