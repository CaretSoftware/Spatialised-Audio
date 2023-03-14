using System;
using UnityEngine;


public class VectorAlignment {
    //public Vector3 vector1;
    //public Vector3 vector2;

    private void Update() {
        
        // Output the results to the console
        // Debug.Log("X-axis alignment: " + Alignment(vector1, vector2));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <param name="right"></param>
    /// <returns>Normalized Dot Product (0 - 1) for x, y, and the general dot product</returns>
    public static Vector3 Alignment(Vector3 vector1, Vector3 vector2) {
        // Calculate alignment for the x-axis
        Vector3 projection1 = Vector3.ProjectOnPlane(vector1, Vector3.up).normalized;
        Vector3 projection2 = Vector3.ProjectOnPlane(vector2, Vector3.up).normalized;

        float alignmentX = Vector3.Dot(projection1, projection2);


        // Calculate alignment for the y-axis
        projection1 = Vector3.ProjectOnPlane(vector1, Vector3.right).normalized;
        projection2 = Vector3.ProjectOnPlane(vector2, Vector3.right).normalized;

        float alignmentY = Vector3.Dot(projection1, projection2);


        // Normalize the alignment values to a range between 0 and 1
        float normalizedAlignmentX = // Mathf.Clamp01
            ((alignmentX + 1f) / 2f);
        float normalizedAlignmentY = // Mathf.Clamp01
            ((alignmentY + 1f) / 2f);

        float z = (Vector3.Dot(vector1, vector2) + 1f) / 2f;

        return new Vector3(normalizedAlignmentX, normalizedAlignmentY, z);
    }
    
    // project the 
    public static Vector3 Alignment(Vector3 vector1, Vector3 vector2, Vector3 right) {
        // Calculate alignment for the x-axis
        Vector3 projection1 = Vector3.ProjectOnPlane(vector1, Vector3.up).normalized;
        Vector3 projection2 = Vector3.ProjectOnPlane(vector2, Vector3.up).normalized;

        float alignmentX = Vector3.Dot(projection1, projection2);


        // Calculate alignment for the y-axis
        projection1 = Vector3.ProjectOnPlane(vector1, right).normalized;
        projection2 = Vector3.ProjectOnPlane(vector2, right).normalized;

        float alignmentY = Vector3.Dot(projection1, projection2);


        // Normalize the alignment values to a range between 0 and 1
        float normalizedAlignmentX = // Mathf.Clamp01
            ((alignmentX + 1f) / 2f);
        float normalizedAlignmentY = // Mathf.Clamp01
            ((alignmentY + 1f) / 2f);

        float z = (Vector3.Dot(vector1, vector2) + 1f) / 2f;

        return new Vector3(normalizedAlignmentX, normalizedAlignmentY, z);
    }

    public static float YAngle(Vector3 aimVector, Vector3 targetVector) {
        // align targetVector with aimVector by rotating it
        Vector3 a = Vector3.ProjectOnPlane(aimVector, Vector3.up);
        Vector3 b = Vector3.ProjectOnPlane(aimVector, Vector3.up);

        Quaternion fromToRotation = Quaternion.FromToRotation(b, a);

        targetVector = fromToRotation * targetVector;

        float normalizedAlignment = (Vector3.Dot(aimVector, targetVector));// + 1f) / 2f;

        return normalizedAlignment;
    }
    
    
    public static Vector3 Alignment(Vector3 vector1) {
        // Calculate alignment for the x-axis
        Vector3 projection1 = Vector3.ProjectOnPlane(vector1, Vector3.up).normalized;
        //Vector3 projection2 = Vector3.ProjectOnPlane(vector2, Vector3.up).normalized;

        float alignmentX = Vector3.Dot(projection1, Vector3.forward);


        // Calculate alignment for the y-axis
        projection1 = Vector3.ProjectOnPlane(vector1, Vector3.right).normalized;
        //projection2 = Vector3.ProjectOnPlane(vector2, Vector3.right).normalized;

        float alignmentY = Vector3.Dot(projection1, Vector3.forward);


        // Normalize the alignment values to a range between 0 and 1
        float normalizedAlignmentX = // Mathf.Clamp01
            ((alignmentX + 1f) / 2f);
        float normalizedAlignmentY = // Mathf.Clamp01
            ((alignmentY + 1f) / 2f);

        float normalizedAlignment = (Vector3.Dot(vector1, Vector3.forward) + 1f) / 2f;

        return new Vector3(normalizedAlignmentX, normalizedAlignmentY, normalizedAlignment);
    }
}