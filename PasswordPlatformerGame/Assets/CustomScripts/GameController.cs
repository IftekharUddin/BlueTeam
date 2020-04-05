using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

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
            string query = url.Split('?')[1];
            // Debug.Log($"Query: {query}");
            // accesses the URL GET fields so as to get the ability to properly query the database
            NameValueCollection queryParts = new NameValueCollection();
            this.ParseQueryString(query, Encoding.UTF8, queryParts);
            string user = queryParts.Get("user") as string;
            // Debug.Log($"User: {user}");
            this.user = user;
            userText.text = this.user;
        }
    }

    private void ParseQueryString(string query, Encoding encoding, NameValueCollection result)
    {
        if (query.Length == 0)
            return;

        var decodedLength = query.Length;
        var namePos = 0;
        var first = true;

        while (namePos <= decodedLength)
        {
            int valuePos = -1, valueEnd = -1;
            for (var q = namePos; q < decodedLength; q++)
            {
                if ((valuePos == -1) && (query[q] == '='))
                {
                    valuePos = q + 1;
                }
                else if (query[q] == '&')
                {
                    valueEnd = q;
                    break;
                }
            }

            if (first)
            {
                first = false;
                if (query[namePos] == '?')
                    namePos++;
            }

            string name;
            if (valuePos == -1)
            {
                name = null;
                valuePos = namePos;
            }
            else
            {
                name = UnityWebRequest.UnEscapeURL(query.Substring(namePos, valuePos - namePos - 1), encoding);
            }
            if (valueEnd < 0)
            {
                namePos = -1;
                valueEnd = query.Length;
            }
            else
            {
                namePos = valueEnd + 1;
            }
            var value = UnityWebRequest.UnEscapeURL(query.Substring(valuePos, valueEnd - valuePos), encoding);

            result.Add(name, value);
            if (namePos == -1)
                break;
        }
    }

    private void makeMultiplier(int multiplier)
    {
        GameObject score = new GameObject();
        score.layer = MaterialController.Instance.FEEDBACK_LAYER;

        MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshR.material = MaterialController.Instance.textMaterial;

        TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
        textM.text = $"{multiplier}x!";
        textM.color = new Color(77f / 255f, 172f / 255f, 38f / 255f);
        textM.characterSize = 0.75f;
        textM.font = MaterialController.Instance.textFont;
        textM.anchor = TextAnchor.MiddleCenter;

        score.transform.position = this.scoreText.transform.position + 5 * Vector3.left;

        score.AddComponent(typeof(BoxCollider2D));

        Rigidbody2D rb = score.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb.mass = 0.001f;
        rb.gravityScale = 0.5f;

        score.AddComponent(typeof(Feedback));
    }

    private void makeAdd(int plus, bool good)
    {
        GameObject score = new GameObject();
        score.layer = MaterialController.Instance.FEEDBACK_LAYER;

        MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshR.material = MaterialController.Instance.textMaterial;

        TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
        textM.text = (good) ? $"+{plus}" : $"{plus}";
        textM.color = (good) ? new Color(77f / 255f, 172f / 255f, 38f / 255f) : new Color(208f / 255f, 28f / 255f, 139f / 255f);
        textM.characterSize = 0.5f;
        textM.font = MaterialController.Instance.textFont;
        textM.anchor = TextAnchor.MiddleCenter;

        score.transform.position = this.scoreText.transform.position + 6 * Vector3.left;

        score.AddComponent(typeof(BoxCollider2D));

        Rigidbody2D rb = score.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb.mass = 0.01f;
        rb.gravityScale = 1.5f;

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
            this.makeAdd(this.goodScore * this.multiplier, true);
        }
        else
        {
            this.streak = 0;
            this.multiplier = 1;
            score += this.badScore;
            this.makeAdd(this.badScore, false);
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
