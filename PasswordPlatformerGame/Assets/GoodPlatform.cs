using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodPlatform : MonoBehaviour
{
    public GameController controller;
    public GameObject passwordHolder;
    private TextMesh password;
    private bool hasCollided = false;

    void Start()
    {
        this.password = this.passwordHolder.GetComponent<TextMesh>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        this.password.color = Color.green;
        if (!this.hasCollided)
        {
            this.hasCollided = true;
            controller.updateScore(100);
        }
    }
}
