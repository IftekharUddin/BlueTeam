using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InstructionScript : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        this.backButton.onClick.AddListener(GoBack);
    }

    private void GoBack()
    {
        SceneManager.LoadSceneAsync("StartMenu");
    }
}
