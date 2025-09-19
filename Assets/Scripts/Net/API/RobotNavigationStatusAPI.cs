using System;
using Newtonsoft.Json;

using XPlan.Net;

public class RobotNavigationStatusAPI : GetWebRequest
{
    public RobotNavigationStatusAPI(Action<bool> finishAction)
    {
        SetUrl(APIDefine.BaseUrl + "/api/core/slam/v1/localization/pose");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        SendWebRequest((jsonStr) => 
        {
            finishAction?.Invoke(jsonStr != null);
        });
    }
}
