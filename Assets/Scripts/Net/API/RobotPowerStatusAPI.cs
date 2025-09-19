using System;
using Newtonsoft.Json;

using XPlan.Net;

public class PowerStatus
{
    [JsonProperty("batteryPercentage")]
    public int BatteryPercentage { get; set; }

    [JsonProperty("dockingStatus")]
    public string DockingStatus { get; set; }   // e.g., "on_dock" / "off_dock"

    [JsonProperty("isCharging")]
    public bool IsCharging { get; set; }

    [JsonProperty("isDCConnected")]
    public bool IsDCConnected { get; set; }

    [JsonProperty("powerStage")]
    public string PowerStage { get; set; }      // e.g., "running" / "shutdown" / "booting"

    [JsonProperty("sleepMode")]
    public string SleepMode { get; set; }       // e.g., "awake" / "sleeping"
}


public class RobotPowerStatusAPI : GetWebRequest
{
    public RobotPowerStatusAPI(Action<PowerStatus> finishAction)
    {
        SetUrl(APIDefine.BaseUrl + "/api/core/system/v1/power/status");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        SendWebRequest((jsonStr) => 
        {
            finishAction?.Invoke(JsonConvert.DeserializeObject<PowerStatus>((string)jsonStr));
        });
    }
}
