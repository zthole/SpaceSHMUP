using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [Header("UI")]
    public Text currentScoreText;
    public Text highScoreText;

    private int lastCurrentScore = -1;
    private int lastHighScore = -1;

    void Start() {
        UpdateScoreDisplay();    
    }

     void Update() {
        if(lastCurrentScore != Main.currentScore || lastHighScore != Main.highScore) {
            UpdateScoreDisplay();
        }
    }

    void UpdateScoreDisplay() {
        currentScoreText.text = "Score: " + Main.currentScore;
        highScoreText.text = "High Score: " + Main.highScore;

        lastCurrentScore = Main.currentScore;
        lastHighScore = Main.highScore;
    }
}
