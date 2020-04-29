using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public Transform player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>() as SpriteRenderer;

        this.transform.position = new Vector3(player.position.x, Camera.main.transform.position.y + Camera.main.orthographicSize - this.spriteRenderer.size.y / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currPosition = this.transform.position;
        this.transform.position = new Vector3(player.position.x, currPosition.y, 0);

        // the top of the screen should be at my anchor position + size_y/2 - instead we give a little bit of leeway
        if (player.position.y <= currPosition.y + this.spriteRenderer.size.y)
        {
            this.spriteRenderer.enabled = false;
        }
        else
        {
            this.spriteRenderer.enabled = true;
        }
    }
}
