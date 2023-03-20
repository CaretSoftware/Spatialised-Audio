using UnityEngine;

public class GameState : GameLoopBaseState {
    public delegate void MaxRounds();
    public static MaxRounds maxRounds;

    private const string state = nameof(GameState);
    private bool _started;
    private bool _end;

    public GameState() => maxRounds += GameEnd;
    
    ~GameState() => maxRounds -= GameEnd;
    
    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        if (!_started) {
            _started = true;
            SpawnManager.respawnGhost?.Invoke();
        }
    }
    
    public override void Run() {
        
        simplePlayerController.UpdateMe();
        shoot.UpdateMe();

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();
        
        if (_end)
            stateMachine.TransitionTo<EndState>();
    }

    private void GameEnd() {
        _end = true;
    }
    
    public override void Exit() { }
}
