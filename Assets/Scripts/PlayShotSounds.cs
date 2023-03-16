using System;
using UnityEngine;

public class PlayShotSounds : MonoBehaviour {
    public delegate void PlayShot();
    public static PlayShot playShot;
    
    [SerializeField] private AudioSource chargeSound;
    [SerializeField] private AudioSource shotSound;

    private void Awake() {
        playShot += PlaySounds;
    }

    private void OnDestroy() {
        playShot -= PlaySounds;
    }

    private void PlaySounds() {
        chargeSound.Play();
        shotSound.Play();
    }
}
