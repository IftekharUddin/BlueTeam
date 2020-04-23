using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class controls the functionality of the back button on the instruction page.
/// </summary>
public class InstructionScript : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        this.backButton.onClick.AddListener(GoBack);
    }

    private void GoBack()
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
        SceneManager.LoadSceneAsync("StartMenu");
    }
}
