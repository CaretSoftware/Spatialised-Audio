using UnityEngine;

public class PauseState : GameLoopBaseState {
    
    private const string state = nameof(PauseState);
    
    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        Time.timeScale = 0f;
        MenuOverlay.overlay?.Invoke(true, MenuOverlay.Pause);
    }
    
    public override void Run() {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<EnterState>();
        
        if (Input.GetKeyDown(KeyCode.F12))
            HeadMovementReader.ShowHeadMovement?.Invoke(true);
    }

    public override void Exit() {
        Time.timeScale = 1f;
        MenuOverlay.overlay?.Invoke(false, MenuOverlay.Play);
    }
}