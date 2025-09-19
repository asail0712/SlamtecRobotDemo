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
        DirectCallUI<string>(UICommand.AddMessage, $"[Net Error] {apiName} happen {error}, becaz {errorContent}");
    }
}
