using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Change : MonoBehaviour
{
    [SerializeField] private int numberChange = 10;
    [SerializeField] private TMP_Text numberChangetxt;

    private void Start()
    {
        numberChangetxt.text = "Change " + numberChange.ToString();
    }
    public void change()
    {
        
        if (numberChange == 0) {
            AudioManager.instance.PlaySFX("e-oh");
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
            int index1 = Random.Range(0, sprites.Count);
            int index2 = Random.Range(0, sprites.Count);
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
        numberChangetxt.text = "Change " + numberChange.ToString();
        AudioManager.instance.PlaySFX("change");
        Debug.Log(sprites.Count);
    }
}
