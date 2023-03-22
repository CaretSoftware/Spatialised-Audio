using System;
using UnityEngine;

public class EnterState : GameLoopBaseState {
    private const string state = nameof(EnterState);
    private bool _tutorial;
    private float _timer;
    private float _timeToTutorial = 2f;

    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
    }
    
    public override void Run() {
        _timer += Time.deltaTime;
        
        simplePlayerController.UpdateMe();
        
        if (Input.GetKeyDown(KeyCode.F12))
            HeadMovementReader.ShowHeadMovement?.Invoke(true);
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();
        
        if (!_tutorial && _timer >= _timeToTutorial)
            _tutorial = true;
        
        if (_tutorial)
            stateMachine.TransitionTo<TutorialState>();
    }
    
    public override void Exit() { }
}
