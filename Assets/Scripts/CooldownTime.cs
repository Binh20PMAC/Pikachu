using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CooldownTime : MonoBehaviour
{
    [SerializeField] Image Cooldown;
    [SerializeField]
    private float Times = 5f;
    private void Update()
    {
        Cooldown.fillAmount -= 1 / Times * Time.deltaTime;
        if (Cooldown.fillAmount == 0 )
        {
            Map.score = 0;
            SceneManager.LoadScene("SampleScene");
        }
    }
}
