using System;
using System.Data.Common;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class TutorialState : GameLoopBaseState {
    public delegate void VoiceDone();
    public static VoiceDone voiceDone;
    
    public delegate void HasOpenedDoor();
    public static HasOpenedDoor openedDoor;
    
    public delegate void PlayVoice(int index);
    public static PlayVoice playVoice;

    public delegate void Hits(int number);
    public static Hits hits;

    private const string state = nameof(TutorialState);
    private bool _drawGun;
    private bool _armed;
    private bool _tutorialComplete;
    private int _hits;
    private bool _pull;
    private float _secondsArmed;

    private bool _voiceDone = true;
    private int _tutorialSequence;

    private LayerMask _layerMaskDefaultTargetPractice;
    private LayerMask _target;

    private bool _openedDoor;

    private Vector3[] kitchenBounds = new Vector3[] {
        new Vector3(4.3f, 2.8f, -4.2f),
        new Vector3(-1.3f, -.1f, -10f),
    };
    
    public TutorialState() {
        voiceDone += VoiceDonePlaying;
        hits += HasHitTarget;
        openedDoor += OpenedDoor;
    }

    private void OpenedDoor() {
        _openedDoor = true;
    }
    
    ~TutorialState() {
        voiceDone -= VoiceDonePlaying;
        hits -= HasHitTarget;
        openedDoor -= OpenedDoor;
    }


    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        LayerMask.NameToLayer("Default");
        _target = LayerMask.NameToLayer("TargetPractice");
        _layerMaskDefaultTargetPractice = LayerMask.GetMask("TargetPractice", "Default");
    }
    
    public override void Run() {
        TutorialSequence();
        simplePlayerController.UpdateMe();

        if (_armed && shoot.UpdateMe(false)) {
            if (HitTrainingTarget())
                TargetPractise.gotHit?.Invoke();
            else if (_hits > 0)
                TargetPractise.flashTarget?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.F12))
            HeadMovementReader.ShowHeadMovement?.Invoke(true);
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();

        if (_tutorialComplete)
            stateMachine.TransitionTo<GameState>();
    }
    
    private void TutorialSequence() {
        if (!_voiceDone) return;
        
        switch (_tutorialSequence) {
            case 0:
                _voiceDone = false;
                playVoice?.Invoke(0); // TODO
                Subtitles.textSubtitles.Invoke("Welcome. To calibrate the head tracking, please turn your head to look straight ahead at the screen and press F12.");
                _tutorialSequence++;
                break;    
            case 1:
                Subtitles.showSubtitles?.Invoke(false);
                if (Input.GetKeyDown(KeyCode.F12))
                    _tutorialSequence++;
                break;
            case 2:
                _voiceDone = false;
                playVoice?.Invoke(1); // TODO
                Subtitles.textSubtitles.Invoke("Thank you!");
                // "Thank you!"
                _tutorialSequence++;
                break;
            case 3:
                if (!InsideKitchen()) {
                    _voiceDone = false;
                    playVoice?.Invoke(2); // TODO
                    Subtitles.textSubtitles.Invoke("Please move towards the kitchen area when you're ready for today's ghost hunt.");
                    // "Please move towards the kitchen area when you're ready for today's ghost hunt."
                }
                _tutorialSequence++;
                break;
            case 4:
                Subtitles.showSubtitles?.Invoke(false);
                if (InsideKitchen())
                    _tutorialSequence++;
                break;
            case 5:
                _voiceDone = false;
                stateMachine.TransitionTo<WeaponDrawState>();
                _armed = true;
                playVoice?.Invoke(3);
                Subtitles.textSubtitles.Invoke("On these older Mark 4's an LED light indicates when they're ready to fire.");
                // "On these older Mark 4's an LED light indicates when they're ready to fire."
                _tutorialSequence++;
                break;
            case 6:
                _voiceDone = false;
                TargetPractise.pullTarget?.Invoke();
                OffScreenArrowIndicator.setImage?.Invoke(OffScreenArrowIndicator.TargetSprite);
                playVoice?.Invoke(4);
                Subtitles.textSubtitles.Invoke("Deploying Virtual Practice Targets!");
                OffScreenArrowIndicator.showArrow?.Invoke(true);
                // "Deploying Virtual Practice Targets!"
                _tutorialSequence++;
                break;
            case 7:
                Subtitles.showSubtitles?.Invoke(false);
                if (_hits >= 1)
                    _tutorialSequence++;
                break;
            case 8:
                _voiceDone = false;
                playVoice?.Invoke(5);
                Subtitles.textSubtitles.Invoke("Remember, ghosts will remain invisible until fired upon. The only way of finding them is through the Heartbeat Sonar©.");
                // "Remember, ghosts will remain invisible until fired upon."
                // "The only way of finding them is through the Heartbeat Sonar."
                GhostAudio.playAudio.Invoke(GhostAudio.Clip.HeartBeat);
                _tutorialSequence++;
                break;
            case 9:
                Subtitles.showSubtitles?.Invoke(false);
                if (_hits >= 2)
                    _tutorialSequence++;
                break;
            case 10:
                if (!_openedDoor) {
                    _voiceDone = false;
                    playVoice?.Invoke(6);
                    Subtitles.textSubtitles.Invoke("You cannot shoot through glass or walls.\nOpen doors with E...");
                }
                // "You cannot shoot through glass or walls."
                // "Open doors with E..."
                _tutorialSequence++;
                break;
            case 11:
                Subtitles.showSubtitles?.Invoke(false);
                if (_hits >= 3)
                    _tutorialSequence++;
                break;
            case 12:
                _voiceDone = false;
                playVoice?.Invoke(7);
                Subtitles.textSubtitles.Invoke("Good job on the tutorial! To start the live test, press enter whenever you're ready.");
                _tutorialSequence++;
                break;
            case 13: 
                Subtitles.showSubtitles?.Invoke(false);
                if (Input.GetKeyDown(KeyCode.Return))
                    _tutorialSequence++;
                break;
            case 14:
                _voiceDone = false;
                playVoice?.Invoke(8);
                Subtitles.textSubtitles.Invoke("Ghosts incoming soon. Be precise, do not miss. Remember you will be timed for your performance.");
                // "Ghosts incoming soon. Be precise, do not miss."
                // "Remember you will be timed for your performance //, starting after your next shot."
                _tutorialSequence++;
                break;
            case 15:
                Subtitles.showSubtitles?.Invoke(false);
                OffScreenArrowIndicator.setTarget?.Invoke(null);
                OffScreenArrowIndicator.showArrow?.Invoke(true);
                OffScreenArrowIndicator.setImage?.Invoke(OffScreenArrowIndicator.GhostSprite);
                HeadMovementReader.resetMovement?.Invoke();
                _tutorialSequence++;
                Shoot.randomHeadTrack?.Invoke();
                _tutorialComplete = true;
                break;
            default:
                break;
        }

        bool InsideKitchen() {
            Vector3 pos = PlayerTransform.PTransform.position;
            if (pos.x > kitchenBounds[0].x) return false;
            if (pos.x < kitchenBounds[1].x) return false;
            if (pos.y > kitchenBounds[0].y) return false;
            if (pos.y < kitchenBounds[1].y) return false;
            if (pos.z > kitchenBounds[0].z) return false;
            if (pos.z < kitchenBounds[1].z) return false;
            return true;
        }
    }

    private void VoiceDonePlaying() {
        _voiceDone = true;
    }
    
    private bool HitTrainingTarget() {
        Vector3 screenCenter = new Vector3(.5f, .5f, 0f);
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(screenCenter);

        Physics.Raycast(ray, out RaycastHit hitInfo, 100, _layerMaskDefaultTargetPractice);

        return hitInfo.transform.gameObject.layer == _target;
    }

    private void HasHitTarget(int number) => _hits = number;
    
    public override void Exit() { }
}