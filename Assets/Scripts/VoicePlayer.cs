using System;
using UnityEngine;
using System.Collections;

public class VoicePlayer : MonoBehaviour {
    public AudioClip[] clips;
    [SerializeField] private AudioSource audioSource;

    private void Awake() {
        TutorialState.playVoice += PlayVoice;
    }

    private void OnDestroy() {
        TutorialState.playVoice -= PlayVoice;
    }

    private void PlayVoice(int index) {
        StartCoroutine(Play(index));
    }
    
    private IEnumerator Play(int index) {
        audioSource.clip = clips[index];
        audioSource.Play();
        yield return new WaitUntil(() => !audioSource.isPlaying);
        TutorialState.voiceDone?.Invoke();
    }
}
