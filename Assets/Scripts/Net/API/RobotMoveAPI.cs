using Newtonsoft.Json;
using System;
using UnityEngine;
using XPlan.Net;
using XPlan.UI;
using static UnityEngine.GraphicsBuffer;

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

public class Target
{
    [JsonProperty("x")]
    public float X { get; set; }
    [JsonProperty("y")]
    public float Y { get; set; }
    [JsonProperty("z")]
    public float Z { get; set; }
}

public class Options
{
    [JsonProperty("target")]
    public Target target { get; set; }
}


public class MoveRequest
{    
    [JsonProperty("action_name")]
    public string actionName { get; set; }

    [JsonProperty("options")]
    public Options options { get; set; }

    [JsonProperty("move_options")]
    public MoveOptions moveOptions { get; set; }
}

public class MoveOptions
{
    public int mode { get; set; }
    public string[] flags { get; set; }
    public float yaw { get; set; }
    public float acceptable_precision { get; set; }
    public int fail_retry_count{ get; set; }
    public float speed_ratio { get; set; }

    public MoveOptions(float yaw)
    {
        this.mode                   = 0;
        this.flags                  = new string[1];
        this.yaw                    = yaw;
        this.acceptable_precision   = 0f;
        this.fail_retry_count       = 0;
        this.speed_ratio            = 0f;
    }
}

public class RobotMoveAPI : PostWebRequest
{
    public RobotMoveAPI(Pose pose, Action<MoveResponse> finishAction)
    {
        UISystem.DirectCall<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotRequest, $"機器人移動"));

        MoveRequest request = new MoveRequest()
        {
            actionName  = "slamtec.agent.actions.MoveToAction",
            options     = new Options()
            {
                target = new Target()
                { 
                    X = pose.X,
                    Y = pose.Y,
                }
            },

            moveOptions = new MoveOptions(pose.Yaw),
        };


        //string jsonBody = $@"{{
        //    ""action_name"": ""slamtec.agent.actions.MoveToAction"",
        //    ""options"": {{
        //        ""target"": {{
        //            ""x"": ""{pose.X}"",
        //            ""y"": ""{pose.Y}"",
        //            ""z"": ""0"",
        //        }},
        //        ""move_options"": {{
        //            ""mode"": 0,
        //            ""flags"": [],
        //            ""yaw"": ""{pose.Yaw}"",
        //            ""acceptable_precision"": 0,
        //            ""fail_retry_count"": 0,
        //            ""speed_ratio"": 0
        //        }}
        //    }}
        //}}";

        string jsonBody = JsonConvert.SerializeObject(request);

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
