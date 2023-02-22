using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class HeapTest : MonoBehaviour {
    private Heap<int> heap;
    private System.Random rand = new System.Random(1234);
    public bool sucess;
        
    Stopwatch _stopwatch = new Stopwatch();
    
    private void Start() {
        _stopwatch.Start();
        
        heap = new Heap<int>(10000);

        for (int i = 0; i < 10000; i++) {
            heap.Insert(rand.Next());
        }

        int min = int.MinValue;

        bool working = true;
        for (int i = 0; i < 10000 && !heap.Empty(); i++) {
            int next = heap.DeleteMin();
            if (next < min) {
                Debug.Log($"{next} {min}");
                working = false;
            }
            min = next;
        }
        
        _stopwatch.Stop();
        Debug.Log($"{working} {_stopwatch.ElapsedMilliseconds}");
    }
}
