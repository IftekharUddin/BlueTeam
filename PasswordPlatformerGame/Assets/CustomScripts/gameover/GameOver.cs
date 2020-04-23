using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

/// <summary>
/// This class controls the functionality for game over, including allowing the user to replay the game 
/// or quit and takes care of updating their score in the database.
/// </summary>
public class GameOver : MonoBehaviour
{

    // the UI element which holds the user's final score
    public Text scoreText;
    // score received from PlayerPrefs
    private int score;
    // the user playing the game
    private string user;
    // UI buttons to restart or quit
    public Button restartButton, quitButton;

    void Start()
    {
        this.restartButton.onClick.AddListener(Restart);
        this.quitButton.onClick.AddListener(Quit);
    }

    void OnEnable()
    {
        // PlayerPrefs is the preferred way to pass (simple) data between scenes
        // https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
        this.score = PlayerPrefs.GetInt("score");
        this.user = PlayerPrefs.GetString("user");

        this.scoreText.text = $"Your Score: {this.score}";

        StartCoroutine(SendScore());
    }

    void Restart()
    {
        // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
        SceneManager.LoadSceneAsync("SelectLevel");
    }

    void Quit()
    {
        // these are C# preprocessor directives (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/) 
        // used to control the functionality of the Quit button 
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log(this.name+" : "+this.GetType()+" : "+System.Reflection.MethodBase.GetCurrentMethod().Name); 
#endif
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
        Application.Quit();
#elif (UNITY_WEBGL)
        Application.OpenURL("about:blank");
#endif
    }

    IEnumerator SendScore()
    {
        // this is an asynchronous function to update the user's score in the database

        // start form to add variables to send with our post request
        WWWForm form = new WWWForm();

        // add user's onyen and score to the form
        form.AddField("onyen", this.user);
        form.AddField("score", this.score);

        // this probably will not go through if you are not running on the server (b/c of SSO issues), but it will not crash the game
        string host = "https://games.fo.unc.edu";

        // see: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html
        UnityWebRequest sendScoreRequest = UnityWebRequest.Post($"{host}/sqlconnect/games/updatePlatformerScore.php", form);
        yield return sendScoreRequest.SendWebRequest();

        // note that this will not fail out if it is forbidden access - future point for teams
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
