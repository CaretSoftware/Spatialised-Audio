using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class CSVWriter : MonoBehaviour {
    private const string folderName = "/Data";
    private string filename = string.Empty;

    private void Start() {
        Directory.CreateDirectory(Application.dataPath + folderName); // does not create directory if exists
        
        int fileNumber = 0;
        
        do {
            filename = Application.dataPath + $"/Data/test{(fileNumber < 10 ? ($"0{fileNumber}") : fileNumber)}.csv";

            fileNumber++;
            
        } while(File.Exists(filename)); // don't overwrite existing files
        
        CreateCSV();

        
        string[] data = new string[]{ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        AppendCSV(data);

        data = new string[]{ "1", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        AppendCSV(data);

        data = new string[]{ "2", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        AppendCSV(data);
    }

    private bool CreateCSV() {
        if (true) { // there is data to write
            TextWriter tw = new StreamWriter(filename, false); // append false
            tw.WriteLine("Test Subject Nr, Total Elapsed Time, Shot Nr, TimeTaken, Precision Total, Precision X, Precision Y, Distance, Hit/Miss, Player Position Spawn, Ghost Position Spawn, Player Position Shot, Ghost Position Shot, Player Floor Shot, Ghost Floor Shot");
            
            tw.Close();
        }
        return true;
    }

    private bool AppendCSV(string[] data) {
        TextWriter tw = new StreamWriter(filename, true); // append true

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
