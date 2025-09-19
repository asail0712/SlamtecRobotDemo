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

        AppendData(jsonBody);

        SendWebRequest((jsonStr) => 
        {
            finishAction?.Invoke(JsonConvert.DeserializeObject<MoveResponse>((string)jsonStr));
        });
    }
}
