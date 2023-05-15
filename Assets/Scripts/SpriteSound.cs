using UnityEngine;
using UnityEngine.UI;

public class SpriteSound : MonoBehaviour
{
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;
    [SerializeField] private Image sound;
    private void Start()
    {
        if (PlayerPrefs.GetInt("SFXMute") == 1)
        {
            sound.sprite = off;
        }
        else
        {
            sound.sprite = on;
        }
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("SFXMute") == 1)
        {
            sound.sprite = off;
        }
        else
        {
            sound.sprite = on;
        }
    }
}
