using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private TMP_Text level;
    private void Update()
    {
        level.text = "Level " + Map.level.ToString();
    }
}
