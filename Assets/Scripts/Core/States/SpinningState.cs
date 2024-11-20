public class SpinningState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        gameManager.spinButton.ToggleButton(false);
        gameManager.stopButton.ToggleButton(true);

        gameManager.board.StartSpinning();
    }

    public void ExitState(GameManager gameManager)
    {

    }
}
