using UnityEngine;

public class WeaponDrawState : GameLoopState {
    private const string state = nameof(WeaponDrawState);
    private static readonly int Raise = Animator.StringToHash("Raise");
    private bool _animationDone = false;

    public WeaponDrawState() => WeaponLiftDone.weaponAnimationDone += AnimationDone;
    
    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        weaponLiftAnimator.SetBool(Raise, true);
    }

    private void AnimationDone() {
        _animationDone = true;
        weaponLiftAnimator.enabled = false;
    }
    
    public override void Run() {
        
        simplePlayerController.UpdateMe();
        
        if (_animationDone)
            stateMachine.TransitionTo<TutorialState>();
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();
    }

    public override void Exit() { }

    ~WeaponDrawState() => WeaponLiftDone.weaponAnimationDone -= AnimationDone;
}
