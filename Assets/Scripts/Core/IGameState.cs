public enum GameState
{
    Initialize,
    Spinning,
    WaitingInput,
    MatchingTile,
    Result,
}
public interface IGameState
{
    void EnterState(GameManager gameManager);
    void ExitState(GameManager gameManager);
}
