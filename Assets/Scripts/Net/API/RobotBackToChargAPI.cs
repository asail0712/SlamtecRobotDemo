using System;
using Newtonsoft.Json;

using XPlan.Net;

public class RobotBackToChargAPI : PostWebRequest
{
    public RobotBackToChargAPI(Action<MoveResponse> finishAction)
    {
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
