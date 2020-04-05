using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public InputField nameField;
    public InputField scoreField;

    public Button submitButton;

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

   public void CallSendScore()
   {
       StartCoroutine(SendScore());
   } 

    IEnumerator SendScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("score", scoreField.text);
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
