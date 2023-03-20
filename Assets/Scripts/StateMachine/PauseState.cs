using UnityEngine;

public class PauseState : GameLoopBaseState {
    
    private const string state = nameof(PauseState);
    
    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        Time.timeScale = 0f;
    }
    
    public override void Run() {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<EnterState>();
    }

    public override void Exit() {
        Time.timeScale = 1f;
    }
}