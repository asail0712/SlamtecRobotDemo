using System;
using Newtonsoft.Json;

using XPlan.Net;

// POI 的資料結構
public class Pose
{
    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    [JsonProperty("yaw")]
    public float Yaw { get; set; }
}

public class Metadata
{
    [JsonProperty("display_name")]
    public string displayName;
}

public class Poi
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("metadata")]
    public Metadata metadata { get; set; }

    [JsonProperty("pose")]
    public Pose Pose { get; set; }
}

public class GetPOIsAPI : GetWebRequest
{
    public GetPOIsAPI(Action<Poi[]> finishAction)
    {
        SetUrl(APIDefine.BaseUrl + "/api/core/artifact/v1/pois");

        AddHeader("accept", "application/json");
        AddHeader("Content-Type", "application/json");

        SendWebRequest((jsonStr) => 
        {
            finishAction?.Invoke(JsonConvert.DeserializeObject<Poi[]>((string)jsonStr));
        });
    }
}
