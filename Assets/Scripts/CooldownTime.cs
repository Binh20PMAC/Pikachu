using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CooldownTime : MonoBehaviour
{
    [SerializeField] private Image cooldown;
    [SerializeField] public static float levelUp = 0f;
    [SerializeField] private float time = 300f;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gamePlay;
    private void Update()
    {
        if (levelUp == 1f)
        {
            cooldown.fillAmount = 1;
        }
        cooldown.fillAmount -= 1 / time * Time.deltaTime;
        levelUp = cooldown.fillAmount;
        if (cooldown.fillAmount == 0)
        {
            gameOver.SetActive(true);
            gamePlay.SetActive(false);
            //if(!gameOver.activeInHierarchy)
            //{
            //    Map.score = 0;
            //    Map.COL = 0;
            //    Map.ROW = 0;
            //    Map.level = 1;
            //    SceneManager.LoadScene("SampleScene");
            //}
        }

    }
}
