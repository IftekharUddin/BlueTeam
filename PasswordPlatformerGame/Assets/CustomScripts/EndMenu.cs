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

    public Text debugText;

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

   public void CallSendScore()
   {
       updateText("About to send Coroutine");
       StartCoroutine(SendScore());
   } 

   void updateText(string text){
        string newText = $"{text.ToString()}";
        debugText.text = newText;
   }

    IEnumerator SendScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("score", scoreField.text);
        updateText("Connection Setup");
        UnityWebRequest sendScoreRequest = UnityWebRequest.Post("https://localhost/sqlconnect/sendScore.php", form);
        yield return sendScoreRequest.SendWebRequest();
        updateText("Connection Sent");
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
