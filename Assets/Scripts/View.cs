using TMPro;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text highScore;
    [SerializeField] private TMP_Text highLevel;
    [SerializeField] private TMP_Text level;
    [SerializeField] private Map Map;
    private void Update()
    {
        score.text = $"Score {Map.score}";
        level.text = $"Level {Map.level}";

        HighScore();
        HighLevel();
    }

    private void HighScore()
    {
        highScore.text = $"Highscore: {PlayerPrefs.GetInt("highscore")}";

        if (Map.score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", Map.score);
        }
    }

    private void HighLevel()
    {
        highLevel.text = $"Highest Level: {PlayerPrefs.GetInt("highlevel")}";

        if (Map.level > PlayerPrefs.GetInt("highlevel"))
        {
            PlayerPrefs.SetInt("highlevel", Map.level);
        }
    }
}
