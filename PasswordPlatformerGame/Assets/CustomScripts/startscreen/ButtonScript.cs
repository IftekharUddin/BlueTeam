using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public Button easyButton, mediumButton, hardButton;

    // Start is called before the first frame update
    void Start()
    {
        this.easyButton.onClick.AddListener(Easy);
        this.mediumButton.onClick.AddListener(Medium);
        this.hardButton.onClick.AddListener(Hard);
    }

    void Easy()
    {
        loadPlayScene(DifficultyUtility.Difficulty.EASY);
    }

    void Medium()
    {
        loadPlayScene(DifficultyUtility.Difficulty.MEDIUM);
    }

    private void Hard()
    {
        loadPlayScene(DifficultyUtility.Difficulty.HARD);
    }

    private void loadPlayScene(DifficultyUtility.Difficulty chosen)
    {
        switch (chosen)
        {
            case DifficultyUtility.Difficulty.EASY:
                PlayerPrefs.SetInt("difficulty", 0);
                break;
            case DifficultyUtility.Difficulty.MEDIUM:
                PlayerPrefs.SetInt("difficulty", 1);
                break;
            case DifficultyUtility.Difficulty.HARD:
                PlayerPrefs.SetInt("difficulty", 2);
                break;
            default:
                break;
        }
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
