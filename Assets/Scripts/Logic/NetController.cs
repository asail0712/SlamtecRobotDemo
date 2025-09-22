using UnityEngine;

using XPlan;
using XPlan.Net;

public class NetController : LogicComponent
{
    public NetController(string baseUrl)
    {
        APIDefine.BaseUrl = $"{baseUrl}:1448";

        WebRequestHelper.AddErrorDelegate(ErrorCallback);
    }

    protected override void OnDispose(bool bAppQuit)
    {
        WebRequestHelper.RemoveErrorDelegate(ErrorCallback);
    }

    private void ErrorCallback(string apiName, string error, string errorContent)
    {
        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.NetError, $"API=> {apiName} 發生 {error}"));
    }
}
