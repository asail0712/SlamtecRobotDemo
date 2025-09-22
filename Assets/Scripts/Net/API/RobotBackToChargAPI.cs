using Newtonsoft.Json;
using System;
using XPlan.Net;
using XPlan.UI;

public class RobotBackToChargAPI : PostWebRequest
{
    public RobotBackToChargAPI(Action<MoveResponse> finishAction)
    {
        UISystem.DirectCall<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotRequest, $"回到充電站"));

        string jsonBody = @"{
            ""action_name"": ""slamtec.agent.actions.MultiFloorBackHomeAction"",
            ""options"": {}
        }";

        SetUrl(APIDefine.BaseUrl + "/api/core/motion/v1/actions");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        AppendData(jsonBody);

        SendWebRequest((jsonStr) => 
        {
            finishAction?.Invoke(JsonConvert.DeserializeObject<MoveResponse>((string)jsonStr));
        });
    }
}
