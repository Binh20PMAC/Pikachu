using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text score;
    private void Update()
    {
        score.text = "Score " + Map.score.ToString();
    }
}
