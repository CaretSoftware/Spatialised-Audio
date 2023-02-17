using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceColliderType : MonoBehaviour
{
    public enum Mode { Floorboards, Clean, Stairs }
    public Mode terrainType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetTerrainType()
    {
        string typeString = "";


        switch (terrainType)
        {
            case Mode.Floorboards:
                typeString = "Floorboards";
                break;
            case Mode.Clean:
                typeString = "Clean";
                break;
            case Mode.Stairs:
                typeString = "Stairs";
                break;
            default:
                typeString = "";
                break;
        }
        return typeString;
    }
}
