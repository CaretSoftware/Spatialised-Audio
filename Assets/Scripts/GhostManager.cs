using System;
using UnityEngine;

public class GhostManager : MonoBehaviour {
    public static Transform ActiveGhost;

    [SerializeField] private Transform ghost;

    private void Awake() {
        ActiveGhost = ghost;
    }
}
