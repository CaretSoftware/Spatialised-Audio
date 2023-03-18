using System;
using UnityEngine;

public class PlayWeaponSounds : MonoBehaviour {
    public delegate void PlayShot();
    public static PlayShot playShot;
    public delegate void PlayCharge();
    public static PlayCharge playCharge;
    
    [SerializeField] private AudioSource chargeSound;
    [SerializeField] private AudioSource shotSound;

    private void Awake() {
        playShot += PlayShotSound;
        playCharge += PlayChargeSound;
    }

    private void OnDestroy() {
        playShot -= PlayShotSound;
        playCharge -= PlayChargeSound;
    }

    private void PlayShotSound() {
        shotSound.Play();
    }

    private void PlayChargeSound() {
        chargeSound.Play();
    }
}
