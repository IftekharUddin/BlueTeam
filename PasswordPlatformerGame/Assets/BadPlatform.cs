using UnityEngine;

public class BadPlatform : MonoBehaviour
{
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
            if (timer < 0f)
            {
                Destroy (this.GetComponent<Collider2D>());
                // rb.bodyType = RigidbodyType2D.Dynamic;
                // rb.gravityScale = 1;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!this.hasCollided)
        {
            this.password.color = Color.red;
            this.hasCollided = true;
            GameController.Instance.updateScore(-100);
        }
    }
}
