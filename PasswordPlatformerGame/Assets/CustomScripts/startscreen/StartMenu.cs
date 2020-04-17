using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button playButton, instructionButton;

    void Start()
    {
        this.playButton.onClick.AddListener(Play);
        this.instructionButton.onClick.AddListener(Instructions);
    }

    void Play()
    {
        SceneManager.LoadSceneAsync("SelectLevel");
    }

    void Instructions()
    {
        SceneManager.LoadSceneAsync("Instructions");
    }
}
