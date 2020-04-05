using UnityEngine;

/// <summary>
/// A class which destroys everything with which it comes into contact. 
/// </summary>
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
        Destroy(other);
    }
}
