using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text highScore;
    [SerializeField] private TMP_Text highLevel;
    private void Update()
    {
        score.text = "Score " + Map.score.ToString();

        HighScore();
        HighLevel();
    }

    private void HighScore()
    {
        highScore.text = "Highscore: " + PlayerPrefs.GetInt("highscore").ToString();

        if (Map.score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", Map.score);
        }
    }

    private void HighLevel()
    {
        highLevel.text = "Highest Level: " + PlayerPrefs.GetInt("highlevel").ToString();

        if (Map.level > PlayerPrefs.GetInt("highlevel"))
        {
            PlayerPrefs.SetInt("highlevel", Map.level);
        }
    }
}
