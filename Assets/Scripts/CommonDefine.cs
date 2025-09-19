using UnityEngine;

public class CommonDefine
{
    
}

public static class UIRequest
{
    // AI STT
    static public string MicStart           = "MicStart";
    static public string MicStop            = "MicStop";

    // Robot Move
    static public string POIToMove          = "POIToMove";
    static public string StopMoving         = "StopMoving";
    static public string BackToHomedock     = "BackToHomedock";
}

public static class UICommand
{
    // Robot Move
    static public string NLPToLocation      = "NLPToLocation";
    static public string SetPOIInfo         = "SetPOIInfo";    
}