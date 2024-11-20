using UnityEngine;

public class TileInput : MonoBehaviour
{
    public Tile tile;
    public Vector2Int position;
    private Vector3 initialTouchPosition;

    private void OnMouseDown()
    {
        initialTouchPosition = Input.mousePosition;
    }
    private void OnMouseUp()
    {
        if (!tile.isSwappable)
            return;

        Vector3 finalTouchPosition = Input.mousePosition;
        Vector3 direction = finalTouchPosition - initialTouchPosition;

        if (direction.magnitude > 50f)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    DynamicBoard.Instance.SwapTiles(position, Vector2Int.right); // Sað
                else
                    DynamicBoard.Instance.SwapTiles(position, Vector2Int.left);  // Sol
            }
            else
            {
                if (direction.y > 0)
                    DynamicBoard.Instance.SwapTiles(position, Vector2Int.up);    // Yukarý
                else
                    DynamicBoard.Instance.SwapTiles(position, Vector2Int.down);  // Aþaðý
            }

            if(GameManager.Instance.currentStateType != GameState.MatchingTile)
                GameManager.Instance.ChangeState(GameState.MatchingTile);
        }
    }
}
