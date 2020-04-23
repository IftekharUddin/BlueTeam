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
/// and the current user and performing such actions as ending the game. 
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController _instance;
    // user's current score
    private int score = 0;
    // the UI element which holds the current score
    public Text scoreText;
    // the onyen of the user playing the game
    private string user;
    // the difficulty chosen by the user
    private DifficultyUtility.Difficulty difficulty;

    public enum Score
    {
        GOOD,
        BAD
    }
    // you can easily change these values in one place
    public int goodScore = 100;
    public int badScore = -100;
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
        // DontDestroyOnLoad(this.gameObject);

        string url = Application.absoluteURL as string;
        if (url.Split('?').Length > 1)
        {
            string query = url.Split('?')[1];
            // accesses the URL GET fields so as to get the ability to properly query the database
            NameValueCollection queryParts = new NameValueCollection();
            this.ParseQueryString(query, Encoding.UTF8, queryParts);
            string user = queryParts.Get("user") as string;

            this.user = user;
        }
    }

    private void ParseQueryString(string query, Encoding encoding, NameValueCollection result)
    {
        // taken from: https://gist.github.com/Ran-QUAN/d966423305ce70cbc320f319d9485fa2
        // as alternative to using C# module which was giving problems when building
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

    void OnEnable()
    {
        int difficulty = PlayerPrefs.GetInt("difficulty");
        switch (difficulty)
        {
            case 0:
                this.difficulty = DifficultyUtility.Difficulty.EASY;
                break;
            case 1:
                this.difficulty = DifficultyUtility.Difficulty.MEDIUM;
                break;
            case 2:
                this.difficulty = DifficultyUtility.Difficulty.HARD;
                break;
            default:
                Application.Quit();
                return;
        }
        if (this.difficulty == DifficultyUtility.Difficulty.MEDIUM)
        {
            this.goodScore *= 2;
            this.badScore *= 2;
        }
        else if (this.difficulty == DifficultyUtility.Difficulty.HARD)
        {
            this.goodScore *= 4;
            this.badScore *= 4;
        }
    }

    private void makeMultiplier(int multiplier)
    {
        // tell the user they have earned a multiplier b/c of reaching a streak
        GameObject score = new GameObject();
        score.layer = MaterialController.Instance.FEEDBACK_LAYER;

        MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshR.material = MaterialController.Instance.textMaterial;

        TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
        textM.text = $"{multiplier}x!";
        // accessible "good" color
        textM.color = new Color(77f / 255f, 172f / 255f, 38f / 255f);
        textM.characterSize = 0.75f;
        textM.font = MaterialController.Instance.textFont;
        textM.anchor = TextAnchor.MiddleCenter;

        score.transform.position = this.scoreText.transform.position + 5 * Vector3.left;

        score.AddComponent(typeof(BoxCollider2D));

        Rigidbody2D rb = score.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        // fall slowly!
        rb.mass = 0.001f;
        rb.gravityScale = 0.5f;

        score.AddComponent(typeof(Feedback));
    }

    private void makeAdd(int plus, bool good)
    {
        // make the object which shows how many points the user gained or lost
        GameObject score = new GameObject();
        score.layer = MaterialController.Instance.FEEDBACK_LAYER;

        MeshRenderer meshR = score.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshR.material = MaterialController.Instance.textMaterial;

        TextMesh textM = score.AddComponent(typeof(TextMesh)) as TextMesh;
        textM.text = (good) ? $"+{plus}" : $"-{plus}";
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
        // PlayerPrefs is the preferred way to pass (simple) data between scenes
        // https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
        PlayerPrefs.SetInt("score", this.score);
        PlayerPrefs.SetString("user", this.user);
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
        SceneManager.LoadSceneAsync("GameOver");
    }

}
