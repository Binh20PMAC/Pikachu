using UnityEngine;
using UnityEngine.UI;

public class CountdownTime : MonoBehaviour
{
    [SerializeField] public Image countdown;
    [SerializeField] private float time = 300f;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gamePlay;
    private void Update()
    {

        countdown.fillAmount -= 1 / time * Time.deltaTime;
        if (countdown.fillAmount == 0)
        {
            gameOver.SetActive(true);
            gamePlay.SetActive(false);
        }
    }

    public void SetTime(float time)
    {
        countdown.fillAmount = time;
    }
}
