using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class controls the functionality of the various buttons on the play menu (back or choice of difficulty).
/// </summary>
public class ButtonScript : MonoBehaviour
{
    // UI elements for each of the various buttons
    public Button easyButton, mediumButton, hardButton, backButton;

    // Start is called before the first frame update
    void Start()
    {
        this.easyButton.onClick.AddListener(Easy);
        this.mediumButton.onClick.AddListener(Medium);
        this.hardButton.onClick.AddListener(Hard);
        this.backButton.onClick.AddListener(GoBack);
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

    private void GoBack()
    {
        SceneManager.LoadSceneAsync("StartMenu");
    }

    private void loadPlayScene(DifficultyUtility.Difficulty chosen)
    {
        // PlayerPrefs is the preferred way to pass (simple) data between scenes
        // https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
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
                PlayerPrefs.SetInt("difficulty", 0);
                break;
        }
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
        SceneManager.LoadSceneAsync("Game");
    }
}
