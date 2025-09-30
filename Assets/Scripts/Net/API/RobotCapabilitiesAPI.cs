using System;
using Newtonsoft.Json;

using XPlan.Net;

public class CapabilitiesRepsonse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("enabled")]
    public bool Enabled { get; set; }
}

public class RobotCapabilitiesAPI : GetWebRequest
{
    public RobotCapabilitiesAPI(Action<CapabilitiesRepsonse[]> finishAction)
    {
        SetUrl(APIDefine.BaseUrl + "/api/core/system/v1/capabilities");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        SendWebRequest((jsonStr) =>
        {
            finishAction?.Invoke(JsonConvert.DeserializeObject<CapabilitiesRepsonse[]>((string)jsonStr));
        });
    }
}