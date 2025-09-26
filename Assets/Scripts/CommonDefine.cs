using UnityEngine;

public enum LogType
{
    Log = 0,
    RobotRequest,
    RobotWarning,
    RobotError,
    NetError,
    ClientError,
}

public class LogInfo
{
    public LogType type;
    public string message;

    public LogInfo(LogType type, string message)
    {
        this.type       = type;
        this.message    = message;
    }

    public override string ToString()
    {
        return $"[{type.ToString()}] {message}";
    }
}

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

    // Robot initial
    static public string RobotInitial       = "RobotInitial";

    // IP Check
    static public string ConfirmIP          = "ConfirmIP";

    // POI
    static public string RefreshPOI         = "RefreshPOI";    
}

public static class UICommand
{
    // Robot Move
    static public string RobotReady         = "RobotReady";
    static public string NLPToLocation      = "NLPToLocation";
    static public string SetPOIInfo         = "SetPOIInfo";

    // Message
    static public string AddMessage         = "AddMessage";
}