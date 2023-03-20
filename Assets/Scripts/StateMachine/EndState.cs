
using UnityEngine;

public class EndState : GameLoopBaseState {
    private const string state = nameof(EndState);

    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
    }

    public override void Run() {
        simplePlayerController.UpdateMe();

        if (Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.LeftControl))
            Application.Quit();
    }

    public override void Exit() {
        
    }
}
