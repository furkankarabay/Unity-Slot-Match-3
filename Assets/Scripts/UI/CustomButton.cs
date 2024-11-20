using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler
{
    public enum ButtonType
    {
        Spin,
        Stop
    }

    public SpriteRenderer buttonSprite;
    public ButtonType buttonType;

    private bool interactable;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        switch (buttonType)
        {
            case ButtonType.Spin:
                ClickAnimation();
                StartSpinning();
                break;
            case ButtonType.Stop:
                ClickAnimation();
                StopSpinning();
                break;
        }
    }
    public void ClickAnimation()
    {
        buttonSprite.transform.DOScale(new Vector3(1.75f, 1.75f, 1.75f), 0.1f).OnComplete(() =>
        {
            buttonSprite.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f);
        });
    }
    public void ToggleButton(bool isActive)
    {
        buttonSprite.DOFade(isActive ? 1 : 0.25f, 0.5f);
        interactable = isActive;

    }

    public void StartSpinning()
    {
        bool allRowsAreReady = true;
        foreach (var item in GameManager.Instance.board.spinningRowLists)
        {
            if (!item.hasStopped)
                allRowsAreReady = false;
        }


        if (allRowsAreReady)
        {
            foreach (var item in GameManager.Instance.board.spinningRowLists)
            {
                item.hasStopped = false;
            }

            GameManager.Instance.ChangeState(GameState.Spinning);
        }
    }

    public void StopSpinning()
    {
        bool allRowsAreReady = true;
        foreach (var item in GameManager.Instance.board.spinningRowLists)
        {
            if (!item.isStartedMoving)
                allRowsAreReady = false;
        }


        if (allRowsAreReady)
        {
            foreach (var item in GameManager.Instance.board.spinningRowLists)
            {
                item.isStartedMoving = false;
            }

            GameManager.Instance.ChangeState(GameState.WaitingInput);
        }
    }
}
