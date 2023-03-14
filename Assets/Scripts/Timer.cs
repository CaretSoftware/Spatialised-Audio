using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public delegate void UpdateMinute(float minutes);
    public static UpdateMinute UpdateTimerMinutes;    
    public delegate void UpdateSecond(float seconds);
    public static UpdateSecond UpdateTimerSeconds;
    
    private string[] _nums;
    [SerializeField] private TextMeshProUGUI minuteText;
    [SerializeField] private TextMeshProUGUI secondsText;
    
    private void Awake() {
        UpdateTimerMinutes += UpdateMinutes;
        UpdateTimerSeconds += UpdateSeconds;
        _nums = Nums();
    }

    private void OnDestroy() {
        UpdateTimerMinutes -= UpdateMinutes;
        UpdateTimerSeconds -= UpdateSeconds;
    }

    private string[] Nums() {
        string[] nums = new string[60];
        for (int i = 0; i < _nums.Length; i++) {
            string str = i < 10 ? "0" + i.ToString() : i.ToString();
        }

        return nums;
    }

    private void UpdateMinutes(float minutes) {
        minuteText.text = _nums[(int)minutes];
    }    
    
    private void UpdateSeconds(float seconds) {
        secondsText.text = _nums[(int)seconds];
    }
}
