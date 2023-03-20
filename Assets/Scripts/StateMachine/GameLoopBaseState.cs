using UnityEngine;

public abstract class GameLoopBaseState : State {

    public SimplePlayerController simplePlayerController;
    public Shoot shoot;
    public Animator weaponLiftAnimator;
}