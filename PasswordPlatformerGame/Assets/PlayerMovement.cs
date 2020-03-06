using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    private float horizontalMove = 0f;
    private bool jump = false;
    public GameObject camera;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * this.runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            this.jump = true;
        }
        this.camera.transform.position = new Vector3(this.transform.position.x + 7, this.camera.transform.position.y, this.camera.transform.position.z);
    }

    void FixedUpdate()
    {
        this.controller.Move(horizontalMove * Time.fixedDeltaTime, false, this.jump);
        this.jump = false;
    }
}
