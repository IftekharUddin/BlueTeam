using UnityEngine;

public class BadPlatform : MonoBehaviour
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
            this.password.color = new Color(208f / 255f, 28f / 255f, 139f / 255f);
            this.hasCollided = true;
            GameController.Instance.updateScore(GameController.Score.BAD);

            string result = PasswordGeneration.Instance.EvaluatePassword(this.password.text);

            GameObject score = new GameObject();
            score.layer = MaterialController.Instance.FEEDBACK_LAYER;

            MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshR.material = MaterialController.Instance.textMaterial;

            TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
            textM.text = result;
            textM.color = Color.white;
            textM.characterSize = 0.25f;
            textM.font = MaterialController.Instance.textFont;
            textM.anchor = TextAnchor.MiddleCenter;

            score.transform.position = this.transform.position;

            score.AddComponent(typeof(BoxCollider2D));

            Rigidbody2D rb = score.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            rb.mass = 0.01f;

            score.AddComponent(typeof(Feedback));
        }
    }
}
