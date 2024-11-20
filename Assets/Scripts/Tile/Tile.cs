using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public TileInput tileInput;
    public TileAnimation tileAnimation;
    public TileData tileData;

    [HideInInspector] public Vector2Int position;
    [HideInInspector] public bool isSwapping = false;
    [HideInInspector] public bool isSwappable = false;

    public void Initialize(TileData tileData, Vector2Int position)
    {
        this.tileData = tileData;
        this.position = position;

        tileAnimation.Initialize(this);

        tileInput.position = position;
        tileInput.tile = this;
    }

    public void SetTileScale(Vector3 localScale, Vector3 localPosition)
    {
        transform.localScale = localScale;
        transform.localPosition = localPosition;

    }
}
