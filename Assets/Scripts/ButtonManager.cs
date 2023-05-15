using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Sound[] sfxSound;
    [SerializeField] private int numberChange = 10;
    [SerializeField] private TMP_Text numberChangetxt;
    [SerializeField] private Map Map;

    private void Start()
    {
        numberChangetxt.text = $"Change {numberChange}";
        if (PlayerPrefs.GetInt("SFXMute") == 1)
        {
            sfxSource.mute = true;
        }
        else
        {
            sfxSource.mute = false;
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSound, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound SFX not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        if (sfxSource.mute)
        {
            PlayerPrefs.SetInt("SFXMute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SFXMute", 0);
        }
    }
    public void ChangeMap()
    {

        if (numberChange == 0)
        {
            PlaySFX("e-oh");
            return;
        }

        List<Sprite> sprites = new List<Sprite>();

        for (int i = 1; i < Map.row - 1; i++)
        {
            for (int j = 1; j < Map.col - 1; j++)
            {
                if (Map.mapPikachu[i, j].gameObject.activeInHierarchy)
                {
                    sprites.Add(Map.mapPikachu[i, j].GetComponent<SpriteRenderer>().sprite);
                }

            }
        }

        int count = sprites.Count;

        for (int i = 0; i < sprites.Count; i++)
        {
            int index1 = UnityEngine.Random.Range(0, sprites.Count);
            int index2 = UnityEngine.Random.Range(0, sprites.Count);
            Sprite temp = sprites[index1];
            sprites[index1] = sprites[index2];
            sprites[index2] = temp;
        }

        for (int i = 1; i < Map.row - 1; i++)
        {
            for (int j = 1; j < Map.col - 1; j++)
            {
                if (Map.mapPikachu[i, j].gameObject.activeInHierarchy)
                    Map.mapPikachu[i, j].GetComponent<SpriteRenderer>().sprite = sprites[--count];

            }
        }
        numberChange--;
        numberChangetxt.text = $"Change {numberChange}";
        PlaySFX("change");
        Debug.Log(sprites.Count);
    }
    public void LoadPlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
