using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTeleporter : MonoBehaviour
{
    [SerializeField] private float teleportMultiplier = 3.0f;
    [SerializeField] private Transform offset = null;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPos;
    private Vector3 offsetVector;
    private float spriteWidth;
    void Start()
    {
        if (offset == null)
        {
            offsetVector = Vector3.zero;
        }
        else
        {
            offsetVector = new Vector3(offset.localPosition.x - transform.localPosition.x, 0, 0);
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = spriteRenderer.bounds.size.x;
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (transform.localPosition.x + offsetVector.x > startPos.x + (teleportMultiplier * spriteWidth))
        {
            transform.localPosition = new Vector3(startPos.x - (teleportMultiplier * spriteWidth) - offsetVector.x, startPos.y, startPos.z);
        }
        else if (transform.localPosition.x - offsetVector.x < startPos.x - (teleportMultiplier * spriteWidth))
        {
            transform.localPosition = new Vector3(startPos.x + (teleportMultiplier * spriteWidth) + offsetVector.x, startPos.y, startPos.z);
        }
    }
}
