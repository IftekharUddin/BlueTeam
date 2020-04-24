using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Governs player movement by delegating to the CharacterController2D script.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    public float downSpeed = 100f;
    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    private bool jump = false;

    // Update is called once per frame
    void Update()
    {
        // Input Horizontal Raw => -1 for left, +1 for right
        horizontalMove = Input.GetAxisRaw("Horizontal") * this.runSpeed;

        // this catches the down key only - ensure the Project Settings have "up" removed from the Input Manager
        verticalMove = Input.GetAxisRaw("Vertical") * this.downSpeed;

        //Input.GetButtonDown from Unity - Edit>Project Settings>Input Manager (default is space, but we want Up Arrow as well)
        if (Input.GetButtonDown("Jump"))
        {
            this.jump = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameController.Instance.Reset();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GameController.Instance.GoBack();
        }

        // set camera position to 7 units ahead of the player - rather arbitrary and naive positioning
        Camera.main.transform.position = new Vector3(this.transform.position.x + 7, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    void FixedUpdate()
    {
        this.controller.Move(horizontalMove * Time.fixedDeltaTime, false, this.jump, verticalMove * Time.fixedDeltaTime);
        this.jump = false;
    }
}
