using UnityEngine;

public class BadPlatform : MonoBehaviour
{
    public GameObject passwordHolder;
    public Rigidbody2D rb;
    private TextMesh password;
    private bool hasCollided = false;
    private float timer = 1f;

    private const int FEEDBACK_LAYER = 11;

    // the material used by the TextMesh 
    private Material textMaterial;
    // the font used by the TextMesh - can easily be changed by substituting the file in the Resources folder
    private Font textFont;

    void Start()
    {
        this.password = this.passwordHolder.GetComponent<TextMesh>();

        // load the text material from the Resources folder
        Material[] mats = Resources.LoadAll<Material>("");
        this.textMaterial = mats[0];

        // load the text font from the Resources folder
        Font[] fonts = Resources.LoadAll<Font>("");
        this.textFont = fonts[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (this.hasCollided)
        {
            this.timer -= Time.fixedDeltaTime;
            if (timer < 0f)
            {
                // Destroy (this.GetComponent<Collider2D>());
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
            GameController.Instance.updateScore(GameController.Score.BAD);

            string result = PasswordGeneration.Instance.EvaluatePassword(this.password.text);

            GameObject score = new GameObject();
            score.layer = FEEDBACK_LAYER;

            MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshR.material = this.textMaterial;

            TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
            textM.text = result;
            textM.color = Color.white;
            textM.characterSize = 0.25f;
            textM.font = this.textFont;
            textM.anchor = TextAnchor.MiddleCenter;

            score.transform.position = this.transform.position;

            score.AddComponent(typeof(BoxCollider2D));

            Rigidbody2D rb = score.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            rb.mass = 0.01f;

            score.AddComponent(typeof(Feedback));
        }
    }
}
