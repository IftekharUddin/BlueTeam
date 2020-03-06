using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using System.Web;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    private int score = 0;
    public Text scoreText;
    public Text userText;
    private string user;

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
            NameValueCollection queryParts = HttpUtility.ParseQueryString(url);
            this.user = queryParts.Get("user") as string;
            userText.text = this.user;
        }
    }

    public void updateScore(int addScore)
    {
        score += addScore;
        string newText = $"Score: {score.ToString()}";
        scoreText.text = newText;
    }
}
