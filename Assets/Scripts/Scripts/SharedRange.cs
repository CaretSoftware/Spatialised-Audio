using System;
using Unity.VisualScripting;
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

    private float tot = 1f;
    
    private void OnValidate() {
        AdjustSliders();
        //AboveFullAdjustment();
        
    }

    [ContextMenu("Reset Sliders")]
    private void Reset() {
        tot = 1f;
        a = a2 = tot * .25f;
        b = b2 = tot * .25f;
        c = c2 = tot * .25f;
        d = d2 = tot * .25f;
    }

    private void AdjustSliders() {
        float[] arr1 = { a, b, c, d };
        float[] arr2 = { a2, b2, c2, d2 };
        
        for (int i = 0; i < arr1.Length; i++) {
            if (arr1[i] != arr2[i]) {
                arr1 = AdjustRanges(arr1, i, arr1[i], tot);
                break;
            }
        }
        
        a2 = a = arr1[0];
        b2 = b = arr1[1];
        c2 = c = arr1[2];
        d2 = d = arr1[3];
    }

    // If the changed value is an increase
        // add all values not above max to an array
        // compare the amount increased to an array would cause it to overfill
        // increase those values
    private void AboveFullAdjustment() {
        float[] arr1 = { a, b, c, d };
        int i = 0;
        float maxAllowedValue = 1f;

        while (ValueAboveMax()) {
            if (arr1[i] > maxAllowedValue)
                arr1 = AdjustRanges(arr1, i, maxAllowedValue, tot, 1f);
            i++;
            i %= 4;
        }
        
        bool ValueAboveMax() {
            for (int j = 0; j < arr1.Length; j++) {
                if (arr1[j] > maxAllowedValue) {
                    return true;
                }
            }
            return false;
        }
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
            return DecreaseFromFull(variables, index, newValue, total); // guards against divide by 0 errors
        
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
            if (i == index) continue;
                
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
