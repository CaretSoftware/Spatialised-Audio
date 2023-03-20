using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour {

    [SerializeField] private Animator weaponLiftAnimator;
    
    private EnterState _enterState = new EnterState();
    private GameState _gameState = new GameState();
    private PauseState _pauseState = new PauseState();
    private TutorialState _tutorialState = new TutorialState();
    private WeaponDrawState _weaponDrawState;
    private EndState _endState = new EndState();
    
    private StateMachine _stateMachine;

    private void Start() {

        List<State> states = new List<State>() {
            _enterState,
            _tutorialState,
            _pauseState,
            (_weaponDrawState = new WeaponDrawState()),
            _gameState,
            _endState,
        };

        SimplePlayerController simplePlayerController = FindObjectOfType<SimplePlayerController>();
        Shoot shoot = FindObjectOfType<Shoot>();

        foreach (GameLoopBaseState state in states) {
            state.simplePlayerController = simplePlayerController;
            state.shoot = shoot;
            state.weaponLiftAnimator = weaponLiftAnimator;
        }
        
        _stateMachine = new StateMachine(states);
    }

    private void Update() {
        _stateMachine.Run();
    }
}
