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
    }

    /// <value> Add or subtract a value from the score </value> 
    public void updateScore(int addScore)
    {
        score += addScore;
        string newText = $"Score: {score.ToString()}";
        scoreText.text = newText;
    }
}
