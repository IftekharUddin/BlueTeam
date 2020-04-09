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
        WWWForm form = new WWWForm();
        form.AddField("onyen", "tas127");
        form.AddField("score", this.score);

        string host = (Application.isEditor) ? "localhost:8000" : "https://games.fo.unc.edu";

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
