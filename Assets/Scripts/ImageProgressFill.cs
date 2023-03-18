using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageProgressFill : MonoBehaviour {
    public delegate void ProgressFill();
    public static ProgressFill ProgressFillStart;
    
    [SerializeField] private Image imageFill;
    [SerializeField] private CanvasGroup images;

    private bool _progress;
    
    private void Start() {
        ProgressFillStart += ProgressFiller;
        ProgressFiller();
    }

    private void OnDestroy() {
        ProgressFillStart -= ProgressFiller;
    }
    
    private void ProgressFiller() {
        StopAllCoroutines();
        StartCoroutine(ProgressSequence());
    }

    private IEnumerator ProgressSequence() {
        yield return new WaitForSeconds(1f);
        _progress = false;
        StartCoroutine(Filler());
        yield return new WaitUntil(() => _progress);
        StartCoroutine(FadeOut());
    }

    private IEnumerator Filler() {
        float t = 0f;
        float fill = 0f;
        float stutterTimer = 0f;

        while (t < 1f || fill < 1f) {
            t += Time.unscaledDeltaTime;

            images.alpha = Ease.OutCirc(t);

            stutterTimer -= Time.unscaledDeltaTime;

            if (stutterTimer < .2f) {
                fill += Time.unscaledDeltaTime;
                imageFill.fillAmount = fill;
                if (stutterTimer < 0f)
                    stutterTimer = UnityEngine.Random.Range(0f, .8f);
            } else {
                fill += Time.unscaledDeltaTime * .15f;
                imageFill.fillAmount = fill;
            }
            
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
