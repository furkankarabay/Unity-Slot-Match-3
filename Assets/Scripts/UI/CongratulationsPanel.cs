using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CongratulationsPanel : MonoBehaviour
{
    [Header("References")]
    public TextAnimator textAnimator;
    public SpriteRenderer alphaBackground;
    public SpriteRenderer starSprite;

    private void Awake()
    {
        FitSpriteToCameraView();
    }

    public void FadeIn()
    {
        alphaBackground.gameObject.SetActive(true);

        alphaBackground.DOFade(0.8f, 0.4f).SetDelay(0.3f).OnComplete(() =>
        {
            textAnimator.StartAnimation();
            StartCoroutine(StarAnimation());
        });
    }

    public IEnumerator StarAnimation()
    {
        yield return new WaitForSeconds(1f);

        starSprite.enabled = true;

        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence
            .Append(starSprite.transform.DOScale(2.25f, 0.3f).SetEase(Ease.InOutSine))
            .Append(starSprite.transform.DOScale(1f, 0.25f).SetEase(Ease.InOutSine))
            .Append(starSprite.transform.DOScale(1.5f, 0.25f).SetEase(Ease.InOutSine).OnComplete(()=>
            {
                StartCoroutine(GameManager.Instance.RestartScene());
            }));

    }

    private void FitSpriteToCameraView()
    {
        Camera mainCamera = Camera.main;

        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        alphaBackground.transform.localScale = new Vector3(cameraWidth, cameraHeight, 1);
    }
}
