using Newtonsoft.Json;
using System;
using XPlan.Net;
using XPlan.UI;

public class RobotStopAPI : DelWebRequest
{
    public RobotStopAPI()
    {
        UISystem.DirectCall<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotRequest, $"機器人停止動作"));

        SetUrl(APIDefine.BaseUrl + "/api/core/motion/v1/actions/:current");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        SendWebRequest();
    }
}
