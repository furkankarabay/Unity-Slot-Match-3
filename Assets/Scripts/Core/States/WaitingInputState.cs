public class WaitingInputState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        gameManager.spinButton.ToggleButton(true);
        gameManager.stopButton.ToggleButton(false);

        gameManager.board.StopSpinning(true);

        gameManager.board.SetTilesSwappable(true);
    }

    public void ExitState(GameManager gameManager)
    {

    }
}
