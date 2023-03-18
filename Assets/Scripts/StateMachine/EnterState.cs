using System;
using UnityEngine;

public class EnterState : GameLoopState {
    
    private const string state = nameof(EnterState);
    private bool _tutorial;

    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
    }
    
    public override void Run() {
        
        simplePlayerController.UpdateMe();
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();
        
        if (Input.GetKeyDown(KeyCode.Return)) // TODO change to voice done
            _tutorial = true;
        
        if (_tutorial)
            stateMachine.TransitionTo<TutorialState>();
    }
    
    public override void Exit() { }
}
