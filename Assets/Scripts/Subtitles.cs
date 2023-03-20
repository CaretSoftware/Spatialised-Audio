using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour {
    public delegate void TextSubtitles(string text);
    public static TextSubtitles textSubtitles;
    
    public delegate void ShowSubtitles(bool text);
    public static ShowSubtitles showSubtitles;

    [SerializeField] private TextMeshProUGUI blackBars;
    [SerializeField] private TextMeshProUGUI subtitles;

    private void Awake() {
        textSubtitles += Text;
        showSubtitles += Show;
    }

    private void OnDestroy() {
        textSubtitles -= Text;
        showSubtitles -= Show;
    } 

    private void Text(string text) {
        subtitles.text = text;
        blackBars.text = "<mark=#010101 padding=\"20, 20, 5, 5\">" + text;
        Show(true);
    }

    private void Show(bool show) {
        subtitles.enabled = show;
        blackBars.enabled = show;
    }
}
