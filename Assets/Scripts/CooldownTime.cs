using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CooldownTime : MonoBehaviour
{
    [SerializeField] private Image Cooldown;
    [SerializeField] public static float levelUp = 0f;
    [SerializeField] private float Times = 300f;
    [SerializeField] private GameObject gameOver;
    private void Update()
    {
        if (levelUp == 1f)
        {
            Cooldown.fillAmount = 1;
        }
        Cooldown.fillAmount -= 1 / Times * Time.deltaTime;
        levelUp = Cooldown.fillAmount;
        if (Cooldown.fillAmount == 0)
        {
            gameOver.SetActive(true);
            if(!gameOver.activeInHierarchy)
            {
                Map.score = 0;
                Map.COL = 0;
                Map.ROW = 0;
                Map.level = 1;
                SceneManager.LoadScene("SampleScene");
            }
        }

    }
}
