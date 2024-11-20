    
public class InitializeState : IGameState
{
    public void EnterState(GameManager gameManager)
    {
        gameManager.spinButton.ToggleButton(true);
        gameManager.stopButton.ToggleButton(false);
    }

    public void ExitState(GameManager gameManager)
    {

    }
}
