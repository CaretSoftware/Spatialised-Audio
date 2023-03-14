using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageProgressFill : MonoBehaviour {
    public delegate void ProgressFill();
    public static ProgressFill ProgressFillStart;
    [SerializeField] private Image imageFill;
    [SerializeField] private CanvasGroup images;

    private bool _progress;
    
    private void Start() {
        ProgressFillStart += ProgressFiller;
        
        Invoke(nameof(ProgressFiller), 1f);
    }

    private void OnDestroy() {
        ProgressFillStart -= ProgressFiller;
    }
    
    private void ProgressFiller() {
        StopAllCoroutines();
        StartCoroutine(ProgressSequence());
    }

    private IEnumerator ProgressSequence() {
        _progress = false;
        StartCoroutine(Filler());
        yield return new WaitUntil(() => _progress);
        StartCoroutine(FadeOut());
    }

    private IEnumerator Filler() {
        float t = 0f;
        
        while (t < 1f) {
            images.alpha = Ease.OutCirc(t);
            imageFill.fillAmount = t;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        images.alpha = 1f;
        imageFill.fillAmount = 1f;
        _progress = true;
    }
    
    private IEnumerator FadeOut() {
        float t = 0f;
        
        while (t < 1f) {
            images.alpha = 1f - t;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        images.alpha = 0;
        imageFill.fillAmount = 0;
    }
}
