using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {

        if (spriteRenderer == null)
        {
            return;
        }

        transform.localScale = new Vector3(1, 1, 1);

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y)
        {
            scale *= cameraSize.x / spriteSize.x + 10f;
        }
        else
        {
            scale *= cameraSize.y / spriteSize.y;
        }

        transform.localScale = scale;
    }
}
