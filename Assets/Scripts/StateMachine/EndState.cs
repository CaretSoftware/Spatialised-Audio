using UnityEngine.SceneManagement;
using UnityEngine;

public class EndState : GameLoopBaseState {
    private const string state = nameof(EndState);
    private bool _fade = true;
    private bool _speak = true;
    private float _timer;
    private bool _voiceDone;

    public EndState() {
        TutorialState.voiceDone += VoiceDone;
    }
    
    ~EndState() {
        TutorialState.voiceDone -= VoiceDone;
    }

    private void VoiceDone() {
        _voiceDone = true;
    }
    
    public override void Enter() {
        WeaponBob.atEnding?.Invoke();
        StateChange.stateUpdate?.Invoke(state);
    }

    public override void Run() {
        if (_speak && _timer >= 1f) {
            _speak = false;
            _voiceDone = false;
            TutorialState.playVoice?.Invoke(9);
            Subtitles.textSubtitles.Invoke("Nice effort in there today!\nThank you so much for participating!");
        }
        
        if (_voiceDone)
            Subtitles.showSubtitles?.Invoke(false);

        if (_fade && _timer >= 2f) {
            _fade = false;
            MenuOverlay.overlay?.Invoke(true, MenuOverlay.Stop);
        }
        
        _timer += Time.unscaledDeltaTime;
        
        if (Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.LeftControl))
            Application.Quit();
        
        if (Input.GetKey(KeyCode.F1))
            SceneManager.LoadScene(0);
    }

    public override void Exit() { }
}
