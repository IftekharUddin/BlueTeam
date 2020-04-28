using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class which governs the timing of the game (currently set to 90 seconds).
/// </summary>
public class Countdown : MonoBehaviour
{
    private const float TIME_START = 90.0f;
    public Text timeText;

    // time limit set here in seconds
    float timeLeft = 90.0f;

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

    public void Reset()
    {
        this.timeLeft = TIME_START;
    }
}
