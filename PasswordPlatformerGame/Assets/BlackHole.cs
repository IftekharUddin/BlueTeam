using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        this.col = this.GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.gameObject;
        Destroy (other);
    }
}
