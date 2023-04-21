using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CooldownTime : MonoBehaviour
{
    [SerializeField] private Image Cooldown;
    [SerializeField] public static float levelup = 0f;
    [SerializeField]private float Times = 300f;
    private void Update()
    {
        if (levelup == 1f)
        {
            Cooldown.fillAmount = 1;
        }
        Cooldown.fillAmount -= 1 / Times * Time.deltaTime;
        levelup = Cooldown.fillAmount;
        if (Cooldown.fillAmount == 0)
        {
            Map.score = 0;
            Map.COL = 0;
            Map.ROW = 0;
            Map.level = 1;
            SceneManager.LoadScene("SampleScene");
        }

    }
}
