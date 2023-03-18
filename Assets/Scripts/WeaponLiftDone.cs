using UnityEngine;

public class WeaponLiftDone : MonoBehaviour {
    public delegate void Done();
    public static Done weaponAnimationDone;

    public void AnimationDone() {
        weaponAnimationDone?.Invoke();
    }
}
