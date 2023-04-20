using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    [SerializeField] RectTransform Time;
    [SerializeField] RectTransform ScoreAndChange;
    private void Update()
    {
        Vector2 time = Time.anchoredPosition;
        Vector2 scoreAndChange = ScoreAndChange.anchoredPosition;
        if (Screen.width == 960)
        {
            time.x = -125;
            scoreAndChange.x = 20;
            Time.anchoredPosition = time;
            ScoreAndChange.anchoredPosition = scoreAndChange;


        }
        else
        {
            scoreAndChange.x = 60;
            time.x = -160;
            Time.anchoredPosition = time;
            ScoreAndChange.anchoredPosition = scoreAndChange;
        }
    }

}

