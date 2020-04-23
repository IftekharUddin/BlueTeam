using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class controls the functionality of the play and instruction buttons on the start screen.
/// </summary>
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
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
        SceneManager.LoadSceneAsync("SelectLevel");
    }

    void Instructions()
    {
        SceneManager.LoadSceneAsync("Instructions");
    }
}
