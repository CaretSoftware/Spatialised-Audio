using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class TutorialState : GameLoopState {
    public delegate void VoiceDone();
    public static VoiceDone voiceDone;
    
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
    
    public TutorialState() {
        voiceDone += VoiceDonePlaying;
        hits += HasHitTarget;
    }

    ~TutorialState() {
        voiceDone -= VoiceDonePlaying;
        hits -= HasHitTarget;
    }

    private LayerMask _layerMaskDefaultTargetPractice;
    private LayerMask _target;

    public override void Enter() {
        StateChange.stateUpdate?.Invoke(state);
        LayerMask.NameToLayer("Default");
        _target = LayerMask.NameToLayer("TargetPractice");
        _layerMaskDefaultTargetPractice = LayerMask.GetMask("TargetPractice", "Default");
    }
    
    public override void Run() {
        VoiceSequence();
        simplePlayerController.UpdateMe();
        
        if (_armed && shoot.UpdateMe(false) && HitTrainingTarget())
            TargetPractise.gotHit?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            stateMachine.TransitionTo<PauseState>();

        if (_tutorialComplete)
            stateMachine.TransitionTo<GameState>();
    }

    private Vector3[] kitchenBounds = new Vector3[] {
        new Vector3(4.3f, 2.8f, -4.2f),
        new Vector3(-1.3f, -.1f, -10f),
    };
    
    private void VoiceSequence() {
        if (!_voiceDone) return;
        
        switch (_tutorialSequence) {
            case 0:
                _voiceDone = false;
                // Welcome, Please move towards the kitchen area when you're ready for today's ghost hunt.
                playVoice?.Invoke(0);
                _tutorialSequence++;
                break;
            case 1:
                if (InsideKitchen())
                    _tutorialSequence++;
                break;
            case 2:
                _voiceDone = false;
                stateMachine.TransitionTo<WeaponDrawState>();
                _armed = true;
                // On these older Mark 4's an LED light indicates when they're ready to fire.
                playVoice?.Invoke(1);
                _tutorialSequence++;
                break;
            case 3:
                _voiceDone = false;
                TargetPractise.pullTarget?.Invoke();
                // Deploying Virtual Practice Targets!
                playVoice?.Invoke(2);
                _tutorialSequence++;
                break;
            case 4:
                if (_hits >= 1)
                    _tutorialSequence++;
                break;
            case 5:
                _voiceDone = false;
                // Remember, ghosts will remain invisible until fired upon.
                // The only way of finding them is through the Heartbeat Sonar.
                playVoice?.Invoke(3);
                _tutorialSequence++;
                break;
            case 6:
                if (_hits >= 2)
                    _tutorialSequence++;
                break;
            case 7:
                _voiceDone = false;
                // You cannot shoot through glass or walls.
                // Open doors with E...
                playVoice?.Invoke(4);
                _tutorialSequence++;
                break;
            case 8:
                if (_hits >= 3)
                    _tutorialSequence++;
                break;
            case 9:
                _voiceDone = false;
                // Ghosts incoming soon. Be precise, do not miss.
                // Remember you will be timed for your performance, starting after your next shot.
                playVoice?.Invoke(5);
                _tutorialSequence++;
                break;
            case 10:
                // _tutorialComplete = _hits >= 3 && _voiceDone;
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