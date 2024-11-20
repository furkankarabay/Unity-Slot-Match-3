public class ResultState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        gameManager.congratulationsPanel.FadeIn();
        gameManager.board.SetTilesSwappable(false);
    }

    public void ExitState(GameManager gameManager)
    {

    }
}
