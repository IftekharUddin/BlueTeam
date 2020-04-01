using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using System.Web;

/// <summary>
/// A singleton class which serves as a controller for the game, storing such game-wide information as the score 
/// and the current user. This class shall also interact with the database.
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController _instance;
    private int score = 0;
    public Text scoreText;
    public Text userText;
    private string user;

    public enum Score
    {
        GOOD,
        BAD
    }
    private int goodScore = 100;
    private int badScore = -100;
    private int streak = 0;
    private int multiplier = 1;

    private const int FEEDBACK_LAYER = 11;

    // the material used by the TextMesh 
    private Material textMaterial;
    // the font used by the TextMesh - can easily be changed by substituting the file in the Resources folder
    private Font textFont;

    /// <value> The singleton instance of this class which can be accessed by any other GameObject </value>
    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        string url = Application.absoluteURL as string;
        if (url.Split('?').Length > 1)
        {
            // accesses the URL GET fields so as to get the ability to properly query the database
            NameValueCollection queryParts = HttpUtility.ParseQueryString(url);
            this.user = queryParts.Get("user") as string;
            userText.text = this.user;
        }

        // load the text material from the Resources folder
        Material[] mats = Resources.LoadAll<Material>("");
        this.textMaterial = mats[0];

        // load the text font from the Resources folder
        Font[] fonts = Resources.LoadAll<Font>("");
        this.textFont = fonts[0];
    }

    private void makeMultiplier(int multiplier)
    {
        GameObject score = new GameObject();
        score.layer = FEEDBACK_LAYER;

        MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshR.material = this.textMaterial;

        TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
        textM.text = $"{multiplier}x!";
        textM.color = Color.green;
        textM.characterSize = 0.75f;
        textM.font = this.textFont;
        textM.anchor = TextAnchor.MiddleCenter;

        score.transform.position = this.scoreText.transform.position + 5 * Vector3.left;

        score.AddComponent(typeof(BoxCollider2D));

        Rigidbody2D rb = score.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb.mass = 0.01f;

        score.AddComponent(typeof(Feedback));
    }

    /// <value> Add or subtract a value from the score </value> 
    public void updateScore(Score scoreAdd)
    {
        if (scoreAdd == Score.GOOD)
        {
            this.streak++;
            if (this.streak == 5)
            {
                this.multiplier = 2;
                this.makeMultiplier(2);
            }
            else if (this.streak == 10)
            {
                this.multiplier = 4;
                this.makeMultiplier(4);
            }

            score += this.goodScore * this.multiplier;
        }
        else
        {
            this.streak = 0;
            this.multiplier = 1;
            score += this.badScore;
        }

        string newText = $"Score: {score.ToString()}";
        scoreText.text = newText;
    }

    
    public void EndGame()
    {
        Debug.Log("RESTARt");
//        StartCoroutine(Register());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", user);
        form.AddField("score", score);
        WWW www = new WWW("http://https://games.fo.unc.edu/sqlconnect/register.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("User created sucessfully.");
        }
        else 
        {
            Debug.Log("User creation failed. Error #" + www.text);
        }
    }
}
