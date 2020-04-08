using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameOver : MonoBehaviour
{

    public Text scoreText;
    private int score;
    public Button restartButton;

    void Start()
    {
        this.restartButton.onClick.AddListener(Restart);
    }

    void OnEnable()
    {
        this.score = PlayerPrefs.GetInt("score");

        this.scoreText.text = $"Your Score: {this.score}";
    }

    void Restart()
    {
        // Debug.Log("Called");
        StartCoroutine(SendScore());
        SceneManager.LoadSceneAsync("StartScreen");
    }

    IEnumerator SendScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", "tas127");
        form.AddField("score", this.score);

        UnityWebRequest sendScoreRequest = UnityWebRequest.Post("https://games.fo.unc.edu/sqlconnect/sendScore.php", form);
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
