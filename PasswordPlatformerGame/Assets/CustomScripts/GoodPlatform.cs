using UnityEngine;

/// <summary>
/// This class controls what happens when you jump on a good password (text turn green, password evaluation fall from screen).
/// </summary>
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
            // this code determines whether the user collided with the platform above, on the side, or below

            // the normal vector to the collision point
            Vector2 normal = col.GetContact(0).normal;
            // the upwards y component of that vector
            float dotUp = Vector2.Dot(normal, Vector2.up);
            // the absolute value of the y component of that vector
            float upPower = Mathf.Abs(Vector2.Dot(normal, Vector2.up));
            // the absolute value of the x component of that vector
            float dotRight = Mathf.Abs(Vector2.Dot(normal, Vector2.right));
            if (dotRight > upPower || dotUp > 0)
            {
                // this value is reversed (dotUp > 0) b/c for some reason the normal vector is relative to the player's feet
                // NOT the platform
                return;
            }

            // this is the RGB for an accessible "good" color
            this.password.color = new Color(77f / 255f, 172f / 255f, 38f / 255f);
            this.hasCollided = true;

            // communicate with the game controller about the score update
            GameController.Instance.updateScore(GameController.Score.GOOD);

            string result = PasswordGeneration.Instance.EvaluatePassword(this.password.text);

            // set up the feedback GameObject
            GameObject feedbackObject = new GameObject();
            feedbackObject.layer = MaterialController.Instance.FEEDBACK_LAYER;

            MeshRenderer meshR = feedbackObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshR.material = MaterialController.Instance.textMaterial;

            TextMesh textM = feedbackObject.AddComponent(typeof(TextMesh)) as TextMesh;
            textM.text = result;
            textM.color = Color.white;
            textM.characterSize = 0.25f;
            textM.font = MaterialController.Instance.textFont;
            textM.anchor = TextAnchor.MiddleCenter;

            feedbackObject.transform.position = this.transform.position;

            feedbackObject.AddComponent(typeof(BoxCollider2D));

            Rigidbody2D rb = feedbackObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            // fall a little slower!
            rb.mass = 0.01f;

            feedbackObject.AddComponent(typeof(Feedback));
        }
    }
}
