using System;
using Newtonsoft.Json;

using XPlan.Net;
using XPlan.UI;

public class MoveState
{
    /// <summary>
    /// 狀態碼：0=RUNNING, 1=SUCCEEDED, 2=FAILED
    /// </summary>
    [JsonProperty("status")]
    public int Status { get; set; }

    /// <summary>
    /// 結果碼：0=成功，其它=失敗原因代碼
    /// </summary>
    [JsonProperty("result")]
    public int Result { get; set; }

    /// <summary>
    /// 文字描述，通常只有失敗時才會有內容
    /// </summary>
    [JsonProperty("reason")]
    public string Reason { get; set; }
}

public class MoveResponse
{
    /// <summary>
    /// 系統分配的動作 ID，用於後續查詢
    /// </summary>
    [JsonProperty("action_id")]
    public int ActionId { get; set; }

    /// <summary>
    /// 動作名稱，例如 "slamtec.agent.actions.MoveToAction"
    /// </summary>
    [JsonProperty("action_name")]
    public string ActionName { get; set; }

    /// <summary>
    /// 動作目前的階段，例如 "GOING_TO_TARGET"
    /// </summary>
    [JsonProperty("stage")]
    public string Stage { get; set; }

    /// <summary>
    /// 動作的詳細狀態
    /// </summary>
    [JsonProperty("state")]
    public MoveState State { get; set; }
}


public class RobotMoveAPI : PostWebRequest
{
    public RobotMoveAPI(string poiName, Action<MoveResponse> finishAction)
    {
        UISystem.DirectCall<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotRequest, $"機器人移動"));

        string jsonBody = $@"{{
            ""action_name"": ""slamtec.agent.actions.MultiFloorMoveAction"",
            ""options"": {{
                ""target"": {{
                    ""poi_name"": ""{poiName}""
                }},
                ""move_options"": {{
                    ""mode"": 0,
                    ""flags"": [ ""with_yaw"", ""precise"" ],
                    ""yaw"": 1,
                    ""acceptable_precision"": 0,
                    ""fail_retry_count"": 0
                }}
            }}
        }}";

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
