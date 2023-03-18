using UnityEngine;

public abstract class GameLoopState : State {

    public SimplePlayerController simplePlayerController;
    public Shoot shoot;
    public Animator weaponLiftAnimator;
}