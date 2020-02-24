using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadPlatform : MonoBehaviour
{

    public GameController controller;
    public GameObject passwordHolder;
    public Rigidbody2D rb;
    private TextMesh password;
    private bool hasCollided = false;
    private float timer = 1f;

    void Start()
    {
        this.password = this.passwordHolder.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.hasCollided)
        {
            this.timer -= Time.fixedDeltaTime;
            Debug.Log(timer);
            if (timer < 0f)
            {
                Debug.Log("Hello");
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1;
                this.timer = 0;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!this.hasCollided)
        {
            this.password.color = Color.red;
            this.hasCollided = true;
            controller.updateScore(-100);
        }
    }
}
