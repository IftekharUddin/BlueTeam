using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    private enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }
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
        loadPlayScene(Difficulty.EASY);
    }

    void Medium()
    {
        loadPlayScene(Difficulty.MEDIUM);
    }

    private void Hard()
    {
        loadPlayScene(Difficulty.HARD);
    }

    private void loadPlayScene(Difficulty chosen)
    {
        switch (chosen)
        {
            case Difficulty.EASY:
                PlayerPrefs.SetInt("difficulty", 0);
                break;
            case Difficulty.MEDIUM:
                PlayerPrefs.SetInt("difficulty", 1);
                break;
            case Difficulty.HARD:
                PlayerPrefs.SetInt("difficulty", 2);
                break;
            default:
                break;
        }
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
