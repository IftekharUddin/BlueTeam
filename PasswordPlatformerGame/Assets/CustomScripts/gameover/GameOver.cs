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
        SceneManager.LoadSceneAsync("SelectLevel");
    }

    IEnumerator SendScore()
    {
        // start form to add variables to send with our post request
        WWWForm form = new WWWForm();

        // add user's onyen and score to the form
        form.AddField("onyen", this.user);
        form.AddField("score", this.score);

        // debug check. If running locally, you'll connect through localhost and not our server
        // string host = (Application.isEditor) ? "localhost:8000" : "https://games.fo.unc.edu";
        string host = "https://games.fo.unc.edu";

        UnityWebRequest sendScoreRequest = UnityWebRequest.Post($"{host}/sqlconnect/games/updatePlatformerScore.php", form);
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
