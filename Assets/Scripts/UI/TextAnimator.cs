using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    public string text;
    public GameObject letterPrefab;
    public Transform lettersParent;
    public float spacingBetweenLetters;
    public Color letterColor;
    public void StartAnimation()
    {
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
                continue;

            SpawnLetter(text[i], i);

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void SpawnLetter(char letter, int index)
    {
        float letterPosition = spacingBetweenLetters * index;
        letterPosition -= (text.Length + 1 ) / 2;

        GameObject letterObj = Instantiate(letterPrefab, lettersParent);
        letterObj.transform.localPosition = new Vector3(letterPosition, 0);

        if(letterObj.TryGetComponent(out TextMeshPro tmp))
        {
            tmp.color = letterColor;
            tmp.text = letter.ToString();
            tmp.sortingOrder += index;
        }

        AnimateLetter(letterObj);
    }

    public void AnimateLetter(GameObject letterObj)
    {
        letterObj.transform.DOScale(Vector3.one * 2f, 0.15f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            letterObj.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InCubic);
        });

        letterObj.transform.DOLocalMoveY(1, 0.15f).OnComplete(() =>
        {
            letterObj.transform.DOLocalMoveY(0, 0.15f);
        });

    }
}
