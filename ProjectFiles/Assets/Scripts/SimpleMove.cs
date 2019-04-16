using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public float mouseSensitivity = 2.5f;

    private Vector3 moveDirection = new Vector3(1, 0, 0);
    private CharacterController controller;

    public bool noisy = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // let the gameObject fall down
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector3 change = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); ;

        change *= mouseSensitivity;

        float x = transform.rotation.eulerAngles.x;
        float y = transform.rotation.eulerAngles.y;

        x -= change.y;

        y += change.x;

        transform.rotation = Quaternion.Euler(new Vector3(x, y));

        if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection = new Vector3(transform.TransformDirection(moveDirection).x, 0f, transform.TransformDirection(moveDirection).z);
            moveDirection = moveDirection * speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        if (moveDirection.magnitude > 0.1f)
        {
            noisy = true;
        }
        else
        {
            noisy = false;
        }

        // Apply gravity
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
        
        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    CharacterController c = GetComponent<CharacterController>();
        
    //    if(Input.GetKeyDown(KeyCode.W))
    //    {
    //        c.Move(Vector3)
    //    }
    //    if(Input.GetKeyDown(KeyCode.A))
    //    {

    //    }
    //    if(Input.GetKeyDown(KeyCode.S))
    //    {

    //    }
    //    if(Input.GetKeyDown(KeyCode.D))
    //    {

    //    }
    //}
}
