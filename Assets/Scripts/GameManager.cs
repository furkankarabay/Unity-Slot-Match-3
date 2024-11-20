using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private IGameState currentState;
    public GameState currentStateType;

    [Header("References")]

    public CustomButton spinButton;
    public CustomButton stopButton;
    public DynamicBoard board;
    public Background background;
    public CongratulationsPanel congratulationsPanel;

    public static GameManager Instance { get; private set; }

    public IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(1);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        background.StartScrolling();
    }

    public void ChangeState(GameState newState)
    {
        currentState?.ExitState(this);

        currentStateType = newState;
        switch (newState)
        {
            case GameState.Initialize:
                currentState = new InitializeState();
                break;
            case GameState.Spinning:
                currentState = new SpinningState();
                break;
            case GameState.WaitingInput:
                currentState = new WaitingInputState();
                break;
            case GameState.MatchingTile:
                currentState = new MatchingTileState();
                break;
            case GameState.Result:
                currentState = new ResultState();
                break;
        }

        currentState.EnterState(this);
    }
}
