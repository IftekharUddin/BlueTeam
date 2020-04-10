using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameOver : MonoBehaviour
{

    public Text scoreText;
    private int score;
    private string user;
    public Button restartButton;

    void Start()
    {
        this.restartButton.onClick.AddListener(Restart);
    }

    void OnEnable()
    {
        this.score = PlayerPrefs.GetInt("score");
        this.user = PlayerPrefs.GetString("user");

        this.scoreText.text = $"Your Score: {this.score}";

        StartCoroutine(SendScore());
    }

    void Restart()
    {
        SceneManager.LoadSceneAsync("StartScreen");
    }

    IEnumerator SendScore()
    {
        // start form to add variables to send with our post request
        WWWForm form = new WWWForm();

        // add user's onyen and score to the form
        form.AddField("onyen", this.user);
        form.AddField("score", this.score);

        // debug check. If running locally, you'll connect through localhost and not our server
        string host = (Application.isEditor) ? "localhost:8000" : "https://games.fo.unc.edu";

        // TODO1: Check if the user doesn't have a score. Set newHighScore as currScore, set exist to false
        // TODO2: If the user has a score, get that score and set that as prevScore
        // TODO3: Compare that score, if bigger, set newHighScore as true
        // tODO4: Depending on "userExist" and "newHighScore", make post request to add/update score. 
        UnityWebRequest sendScoreRequest = UnityWebRequest.Post($"{host}/db/php/games/UpdateScores.php", form);
        yield return sendScoreRequest.SendWebRequest();

        if (sendScoreRequest.isNetworkError || sendScoreRequest.isHttpError)
        {
            Debug.Log(sendScoreRequest.error);
        }
        else
        {
            Debug.Log("Form Upload Complete!");
        }
    }
}
