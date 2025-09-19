using System;
using Newtonsoft.Json;

using XPlan.Net;

public class RobotWakeUpAPI : PostWebRequest
{
    public RobotWakeUpAPI()
    {
        SetUrl(APIDefine.BaseUrl + "/api/core/system/v1/power/:wakeup");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        SendWebRequest();
    }
}
