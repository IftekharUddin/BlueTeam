using UnityEngine;

public class GoodPlatform : MonoBehaviour
{
    public GameObject passwordHolder;
    private TextMesh password;
    private bool hasCollided = false;

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!this.hasCollided)
        {
            this.password.color = new Color(77f / 255f, 172f / 255f, 38f / 255f);
            this.hasCollided = true;
            GameController.Instance.updateScore(GameController.Score.GOOD);

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
