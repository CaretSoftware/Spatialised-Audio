using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundCounter : MonoBehaviour {
    public delegate void RoundText(int round);
    public static RoundText roundText;

    [SerializeField] private TextMeshProUGUI text;

    private void Awake() {
        roundText += UpdateRoundText;
    }

    private void OnDestroy() {
        roundText -= UpdateRoundText;
    }

    private void UpdateRoundText(int round) {
        text.text = round < 10 ? "0" + round.ToString() : round.ToString();
    }
}
