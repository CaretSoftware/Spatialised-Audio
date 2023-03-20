using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;


public class CSVWriter : MonoBehaviour {
    private const string Rubric = "Test Subject Nr, Total Elapsed Time, Shot Nr, TimeTaken, Precision Total," +
                                  " Precision X, Precision Y, Distance, Hit, Player Floor Shot, Ghost Floor Shot";

    private const string FolderName = "/Data";
    private string _filename = string.Empty;
    public static int SubjectNumber;
    public static int RoundNumber = 1;
    [SerializeField] private bool writeToFile = true;

    private void Start() {
        
#if UNITY_EDITOR
        //writeToFile = false;
#else
        writeToFile = true;
#endif        
        
        Directory.CreateDirectory(Application.dataPath + FolderName); // does not create directory if exists

        SubjectNumber = 0;

        do {
            _filename = Application.dataPath +
                        $"/Data/test{(SubjectNumber < 10 ? ($"0{SubjectNumber}") : SubjectNumber)}.csv";

            SubjectNumber++;
        } while (File.Exists(_filename)); // don't overwrite existing files

        CreateCSV();
    }


    private bool CreateCSV() {
        if (!writeToFile) return false;

        TextWriter tw = new StreamWriter(_filename, false); // append false
        tw.WriteLine(Rubric);

        tw.Close();
        return true;
    }

    public bool AppendCSV(string[] data) {
        if (!writeToFile) return false;

        RoundNumber++;

        TextWriter tw = new StreamWriter(_filename, true); // append true

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < data.Length; i++) {
            sb.Append(data[i]);
            if (i < data.Length - 1)
                sb.Append(",");
        }

        tw.WriteLine(sb);

        tw.Close();

        return true;
    }

    public bool AppendCSV(ShotData data) {
        if (!writeToFile) return false;

        RoundNumber++;

        TextWriter tw = new StreamWriter(_filename, true); // append true

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < data.data.Length; i++) {
            sb.Append(data.data[i]);
            if (i < data.data.Length - 1)
                sb.Append(",");
        }

        tw.WriteLine(sb);

        tw.Close();

        return true;
    }
}

public struct ShotData {
    public ShotData(int subjectNumber, float totalElapsedTime, int roundNumber, float roundElapsedTime,
        float precision, float precisionX, float precisionY, float distance, bool hit, int playerFloor,
        int ghostFloor) {
        _subjectNumber = subjectNumber;
        _totalElapsedTime = totalElapsedTime;
        _roundNumber = roundNumber;
        _roundElapsedTime = roundElapsedTime;
        _precision = precision;
        _precisionX = precisionX;
        _precisionY = precisionY;
        _distance = distance;
        _hit = hit;
        _playerFloor = playerFloor;
        _ghostFloor = ghostFloor;

        data = new string[11];
        data[0] = _subjectNumber.ToString();
        data[1] = _totalElapsedTime.ToString(CultureInfo.InvariantCulture);
        data[2] = _roundNumber.ToString();
        data[3] = _roundElapsedTime.ToString(CultureInfo.InvariantCulture);
        data[4] = _precision.ToString(CultureInfo.InvariantCulture);
        data[5] = _precisionX.ToString(CultureInfo.InvariantCulture);
        data[6] = _precisionY.ToString(CultureInfo.InvariantCulture);
        data[7] = _distance.ToString(CultureInfo.InvariantCulture);
        data[8] = _hit.ToString();
        data[9] = _playerFloor.ToString();
        data[10] = _ghostFloor.ToString();
    }

    public string[] data;
    private int _subjectNumber;
    private float _totalElapsedTime;
    private int _roundNumber;
    private float _roundElapsedTime;
    private float _precision;
    private float _precisionX;
    private float _precisionY;
    private float _distance;
    private bool _hit;
    private int _playerFloor;
    private int _ghostFloor;
}
