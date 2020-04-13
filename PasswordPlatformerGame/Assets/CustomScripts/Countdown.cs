using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public Text timeText;

    // time limit set here in seconds
//    float timeLeft = 45.0f;
    float timeLeft = 45.0f;

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        string newText = $"{timeLeft.ToString("#.0")}";
        timeText.text = newText;
        if (timeLeft < 0)
        {
            GameController.Instance.EndGame();
            Destroy(this.gameObject);
        }
    }
}
