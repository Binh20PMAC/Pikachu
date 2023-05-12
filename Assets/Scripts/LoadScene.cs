using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadPlayAgain()
    {
        Map.score = 0;
        Map.col = 0;
        Map.row = 0;
        SceneManager.LoadScene("SampleScene");
    }
}
