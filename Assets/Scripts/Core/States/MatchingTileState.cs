public class MatchingTileState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        gameManager.spinButton.ToggleButton(false);
        gameManager.stopButton.ToggleButton(false);
    }

    public void ExitState(GameManager gameManager)
    {

    }
}
