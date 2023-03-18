using UnityEngine;

public class GameState : GameLoopState {
    
    private const string state = nameof(GameState);
    private bool _started;
    
    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        if (!_started) {
            _started = true;
            SpawnManager.respawnGhost?.Invoke();
        }
    }
    
    public override void Run() {
        
        simplePlayerController.UpdateMe();
        shoot.UpdateMe(); // if weapon lifted

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();
    }
    
    public override void Exit() { }
}
