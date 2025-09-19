using System;
using Newtonsoft.Json;

using XPlan.Net;

// POI 的資料結構
public class Pose
{
    [JsonProperty("x")]
    public double X { get; set; }

    [JsonProperty("y")]
    public double Y { get; set; }

    [JsonProperty("yaw")]
    public double Yaw { get; set; }
}

public class Poi
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("poi_name")]
    public string DisplayName { get; set; }

    [JsonProperty("type")]
    public string poiType { get; set; }

    [JsonProperty("floor")]
    public string FloorId { get; set; }

    [JsonProperty("building")]
    public string poiBuilding { get; set; }

    [JsonProperty("pose")]
    public Pose Pose { get; set; }
}

public class GetPOIsAPI : GetWebRequest
{
    public GetPOIsAPI(Action<Poi[]> finishAction)
    {
        SetUrl(APIDefine.BaseUrl + "/api/multi-floor/map/v1/pois");

        SendWebRequest((jsonStr) => 
        {
            finishAction?.Invoke(JsonConvert.DeserializeObject<Poi[]>((string)jsonStr));
        });
    }
}
