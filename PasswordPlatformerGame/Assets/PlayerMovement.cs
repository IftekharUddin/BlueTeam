using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    public float downSpeed = 100f;
    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    private bool jump = false;
    public GameObject camera;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * this.runSpeed;

        // this catches the down key only - ensure the Project Settings have "up" removed from the Input Manager
        verticalMove = Input.GetAxisRaw("Vertical") * this.downSpeed;

        //Input.GetButtonDown from Unity - Edit>Project Settings>Input Manager
        if (Input.GetButtonDown("Jump"))
        {
            this.jump = true;
        }

        this.camera.transform.position = new Vector3(this.transform.position.x + 7, this.camera.transform.position.y, this.camera.transform.position.z);
    }

    void FixedUpdate()
    {
        this.controller.Move(horizontalMove * Time.fixedDeltaTime, false, this.jump, verticalMove * Time.fixedDeltaTime);
        this.jump = false;
    }
}
