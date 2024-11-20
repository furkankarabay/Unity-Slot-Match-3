using DG.Tweening;
using UnityEngine;

public class TileAnimation : MonoBehaviour
{
    public Tile tile;
    public SpriteRenderer spriteRenderer;
    public void Initialize(Tile tile)
    {
        transform.localPosition = Vector3.zero;
        this.tile = tile;
    }
    public void OnMatched()
    {
        transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
                 .SetLoops(1, LoopType.Yoyo)
                 .SetEase(Ease.InOutQuad).OnComplete(()=>
                 {

                 });

        transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.15f)
                 .SetLoops(2, LoopType.Yoyo)
                 .SetEase(Ease.OutBounce);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f))
                .AppendInterval(0.2f)
                .Append(transform.DOScale(new Vector3(0f, 0f, 1f), 0.125f))
                .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.125f));

        sequence.Play();
    }

    public void OnSwapped(Tile targetTile)
    {
        tile.isSwapping = true;

        transform.DOMove(targetTile.tileAnimation.gameObject.transform.position, 0.35f).OnComplete(() =>
        {
            tile.isSwapping = false;
        });
    }
}
