using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadSceneAsync("StartScreen");
    }
}
