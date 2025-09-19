using UnityEngine;

using XPlan;

public class NetController : LogicComponent
{
    public NetController(string baseUrl)
    {
        APIDefine.BaseUrl = $"{baseUrl}:1448";
    }
}
