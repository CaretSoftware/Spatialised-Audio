using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuOverlay : MonoBehaviour {
    public delegate void Overlay(bool on, string text);
    public static Overlay overlay;

    public static string Play = "▶";
    public static string Pause = "II";
    public static string Stop = "Thank You!";

    [SerializeField] private float smoothDamp;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private float _currentVelocity;
    private float _alpha;
    private float _targetAlpha;

    private void Awake() {
        overlay += SetOverlay;
    }

    private void OnDestroy() {
        overlay -= SetOverlay;
    }

    private void SetOverlay(bool on, string text) {
        _targetAlpha = on ? 1f : 0f;
        this.text.text = text;
    }

    private void Update() {
        _alpha = Mathf.SmoothDamp(_alpha, _targetAlpha, ref _currentVelocity, smoothDamp, 1f, Time.unscaledTime);
        canvasGroup.alpha = _alpha;
    }
}
