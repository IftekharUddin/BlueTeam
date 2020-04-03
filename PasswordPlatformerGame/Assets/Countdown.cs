using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    // Start is called before the first frame update
    public Text timeText;

    // time limit set here in seconds
    float timeLeft = 10.0f;

    // Update is called once per frame
    void Update()
    {
        timeLeft -=  Time.deltaTime;
        string newText = $"{timeLeft.ToString("#.0")}";
        timeText.text = newText;
        if(timeLeft < 0){
            timeText.text = $"Update";
            FindObjectOfType<GameController>().EndGame();
        }
    }
}
