using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodPlatform : MonoBehaviour
{
    public GameObject passwordHolder;
    private TextMesh password;
    private bool hasCollided = false;

    void Start()
    {
        this.password = this.passwordHolder.GetComponent<TextMesh>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!this.hasCollided)
        {
            this.password.color = Color.green;
            this.hasCollided = true;
            GameController.Instance.updateScore(100);
        }
    }
}
