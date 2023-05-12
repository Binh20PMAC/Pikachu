using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    [SerializeField] RectTransform time;
    [SerializeField] RectTransform scoreAndChange;
    [SerializeField] RectTransform rectTransform;
    private Vector2 timeTemp;
    private Vector2 scoreAndChangeTemp;
    private float height;
    private void Start()
    {
        timeTemp = time.anchoredPosition;
        scoreAndChangeTemp = scoreAndChange.anchoredPosition;

        height = rectTransform.rect.height;
    }
    private void Update()
    {
        if (height == 1080)
        {
            scoreAndChangeTemp.x = 60;
            timeTemp.x = -160;
            time.anchoredPosition = timeTemp;
            scoreAndChange.anchoredPosition = scoreAndChangeTemp;
        }
        else if (height == 1200)
        {
            timeTemp.x = -125;
            scoreAndChangeTemp.x = 20;
            time.anchoredPosition = timeTemp;
            scoreAndChange.anchoredPosition = scoreAndChangeTemp;
        }
        height = rectTransform.rect.height;
    }
}

