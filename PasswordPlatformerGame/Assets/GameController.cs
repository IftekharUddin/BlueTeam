using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static int score = 0;
    public Text scoreText;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public void updateScore(int addScore)
    {
        score += addScore;
        string newText = $"Score: {score.ToString()}";
        Debug.Log(newText);
        this.scoreText.text = newText;
    }
}
