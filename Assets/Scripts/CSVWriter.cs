using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class CSVWriter : MonoBehaviour {
    private const string Rubric = "Test Subject Nr, Total Elapsed Time, Shot Nr, TimeTaken, Precision Total,"+
                                  " Precision X, Precision Y, Distance, Hit/Miss, Player Floor Shot, Ghost Floor Shot";
    private const string FolderName = "/Data";
    private string _filename = string.Empty;
    public static int SubjectNumber;
    public static int RoundNumber = 1;
    [SerializeField] private bool writeToFile = false;

    private void Start() {
        Directory.CreateDirectory(Application.dataPath + FolderName); // does not create directory if exists
        
        SubjectNumber = 0;
        
        do {
            _filename = Application.dataPath + $"/Data/test{(SubjectNumber < 10 ? ($"0{SubjectNumber}") : SubjectNumber)}.csv";

            SubjectNumber++;
            
        } while(File.Exists(_filename)); // don't overwrite existing files
        
        CreateCSV();
        
        string[] data = new string[]{ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        AppendCSV(data);

        data = new string[]{ "1", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        AppendCSV(data);

        data = new string[]{ "2", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        AppendCSV(data);
    }
    


    private bool CreateCSV() {
        if (!writeToFile) return false;
        
        TextWriter tw = new StreamWriter(_filename, false); // append false
        tw.WriteLine(Rubric);
            
        tw.Close();
        return true;
    }

    private bool AppendCSV(string[] data) {
        if (!writeToFile) return false;

        RoundNumber++;

        TextWriter tw = new StreamWriter(_filename, true); // append true

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < data.Length; i++) {
            sb.Append(data[i]);
            sb.Append(",");
        }
        
        tw.WriteLine(sb);

        tw.Close();
        
        return true;
    }
}

public struct ShotData {

    public ShotData(int subjectNumber, float totalElapsedTime, int roundNumber, float roundElapsedTime, 
        float precision, float precisionX, float precisionY, float distance, bool hit, int playerFloor, int ghostFloor) {

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
    }

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
