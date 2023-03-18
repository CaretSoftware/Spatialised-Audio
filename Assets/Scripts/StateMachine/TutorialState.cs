using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class TutorialState : GameLoopState {
    public delegate void Hits(int number);
    public static Hits hits;

    private const string state = nameof(TutorialState);
    private bool _drawGun;
    private bool _armed;
    private bool _tutorialComplete;
    private int _hits;
    private bool _pull;
    private float _secondsArmed;

    public TutorialState() {
        hits += HasHitTarget;
    } 

    ~TutorialState() => hits -= HasHitTarget;

    private LayerMask _layerMaskDefaultTargetPractice;
    private LayerMask _target;

    public override void Enter() {
        Debug.Log(nameof(TutorialState));
        
        StateChange.stateUpdate?.Invoke(state);
        LayerMask.NameToLayer("Default");
        _target = LayerMask.NameToLayer("TargetPractice");
        _layerMaskDefaultTargetPractice = LayerMask.GetMask("TargetPractice", "Default");
    }
    
    public override void Run() {
        
        simplePlayerController.UpdateMe();
        if (_armed) {
            _secondsArmed += Time.deltaTime;

            if (_secondsArmed > 2f)
                TargetPractise.pullTarget?.Invoke();
            
            if (shoot.UpdateMe() && RayCastForTarget())
                TargetPractise.gotHit?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();
        
        if (Input.GetKeyDown(KeyCode.Return)) // HAVE THEM HIT THE TARGET INSTEAD
            _drawGun = true;

        if (_drawGun && !_armed) {
            _drawGun = false;
            _armed = true;
            stateMachine.TransitionTo<WeaponDrawState>();
        }

        _tutorialComplete = _hits >= 3;
        
        if (_tutorialComplete)
            stateMachine.TransitionTo<GameState>();
    }

    private bool RayCastForTarget() {
        Vector3 screenCenter = new Vector3(.5f, .5f, 0f);
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(screenCenter);

        Physics.Raycast(ray, out RaycastHit hitInfo, 100, _layerMaskDefaultTargetPractice);

        return hitInfo.transform.gameObject.layer == _target;
    }

    private void HasHitTarget(int number) => _hits = number;
    
    public override void Exit() { }
}