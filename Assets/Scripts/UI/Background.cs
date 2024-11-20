using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float scrollSpeed = 1f;
    public float scrollDistance = 0.5f;
    private Material material;

    public void StartScrolling()
    {
        material = GetComponent<Renderer>().material;

        material.DOOffset(new Vector2(scrollDistance, scrollDistance), scrollSpeed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
    }
}
