using System;
using UnityEngine;

public class SharedRange : MonoBehaviour {
    [Range(0f, 1f)] public float a = .4f;
    [Range(0f, 1f)] public float b = .3f;
    [Range(0f, 1f)] public float c = .2f;
    [Range(0f, 1f)] public float d = .1f;

    [SerializeField, HideInInspector] private float a2 = .4f;
    [SerializeField, HideInInspector] private float b2 = .3f;
    [SerializeField, HideInInspector] private float c2 = .2f;
    [SerializeField, HideInInspector] private float d2 = .1f;
    
    public float tot = 1f;
    public bool reset = false;
    
    private void OnValidate() {
        if (reset) {
            reset = false;
            a = a2 = .4f;
            b = b2 = .3f;
            c = c2 = .2f;
            d = d2 = .1f;
        }
        
        float[] arr1 = new float[] { a, b, c, d };
        float[] arr2 = new float[] { a2, b2, c2, d2 };
        
        for (int i = 0; i < arr1.Length; i++) {
            if (arr1[i] != arr2[i]) {
                arr1 = AdjustRanges(arr1, i, arr1[i], 1f);
                break;
            }
        }

        a = arr1[0];
        b = arr1[1];
        c = arr1[2];
        d = arr1[3];

        a2 = a;
        b2 = b;
        c2 = c;
        d2 = d;

        tot = a + b + c + d;
    }
    
    // TODO allow > 100%
    // check if any value overfull after the increase
    // if so
    //      calculate the overfill
    //      fill other sliders with (overfill / (sliders - 1)) (can be more than one)
    //          (check so other sliders aren't overfilled with the increase from the overfill)
    //          ?while loop?
    //      while (Overfill(variables, out int index, out float overspill)) {
    //          // overspill := overfill / (sliders - 1)
    //          for ( ... )
    //              add overspill to the other sliders (not the altered slider if possible)
    //      }
    //      

    public float[] AdjustRanges(float[] variables, int index, float newValue, float total = 1f, float full = 1f) {

        if (newValue >= total)  // guard against divide by 0 errors
            return FullValue(variables, index, total);

        float sum = Sum(variables, index);

        if (sum <= 0f)
            return DecreaseFromFull(variables, index, newValue, total); // guard against divide by 0 errors
        
        float k = (total - newValue) / sum;

        for (int i = 0; i < variables.Length; i++) {
            if (i != index)
                variables[i] *= k;
        }

        variables[index] = newValue;
        
        
        return variables;
    }

    private float Sum(float[] variables, int index) {
        float sum = 0f;
        for (int i = 0; i < variables.Length; i++) {
            if (i != index)
                sum += variables[i];
        }

        return sum;
    }

    private float[] DecreaseFromFull(float[] variables, int index, float newValue, float total) {
        float increase = (total - newValue) / (variables.Length - 1);
        
        for (int i = 0; i < variables.Length; i++) {
            if (i != index)
                variables[i] = increase;
        }

        variables[index] = newValue;
        return variables;
    }

    private float[] FullValue(float[] variables, int index, float total = 1f) {
        variables = new float[variables.Length];
        variables[index] = total;
        return variables;
    }
}
