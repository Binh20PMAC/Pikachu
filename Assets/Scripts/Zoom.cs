using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    [SerializeField] RectTransform Time;
    [SerializeField] RectTransform ScoreAndChange;
    [SerializeField] RectTransform rectTransform;
    private Vector2 time;
    private Vector2 scoreAndChange;
    private float height;
    private void Start()
    {
        time = Time.anchoredPosition;
        scoreAndChange = ScoreAndChange.anchoredPosition;

        height = rectTransform.rect.height;
    }
    private void Update()
    {
        if (height == 1080)
        {
            scoreAndChange.x = 60;
            time.x = -160;
            Time.anchoredPosition = time;
            ScoreAndChange.anchoredPosition = scoreAndChange;
        }
        else if (height == 1200)
        {
            time.x = -125;
            scoreAndChange.x = 20;
            Time.anchoredPosition = time;
            ScoreAndChange.anchoredPosition = scoreAndChange;
        }
        height = rectTransform.rect.height;
    }
}

