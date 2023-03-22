using UnityEngine;

public class WeaponLiftDone : MonoBehaviour {
    public delegate void Done();
    public static Done weaponAnimationDone;

    [SerializeField] private Animator animator;

    public void AnimationDone() {
        animator.enabled = false;
        weaponAnimationDone?.Invoke();
    }
}
