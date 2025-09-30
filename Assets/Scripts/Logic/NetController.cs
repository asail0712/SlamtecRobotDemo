using UnityEngine;

using XPlan;
using XPlan.Net;
using XPlan.Observe;

public class RobotInitialMsg : MessageBase 
{
    public RobotInitialMsg()
    {

    }
}


public class NetController : LogicComponent
{
    public NetController(string baseUrl)
    {
        APIDefine.BaseUrl = $"http://{baseUrl}:1448";

        WebRequestHelper.AddErrorDelegate(ErrorCallback);

        AddUIListener<string>(UIRequest.ConfirmIP, (ipStr) => 
        {
            APIDefine.BaseUrl = $"http://{baseUrl}:1448";

            SendGlobalMsg<RobotInitialMsg>();
        });

        DirectCallUI<string>(UICommand.InitIP, baseUrl);
    }

    protected override void OnDispose(bool bAppQuit)
    {
        WebRequestHelper.RemoveErrorDelegate(ErrorCallback);
    }

    private void ErrorCallback(string apiUrl, string error, string errorContent)
    {
        if(apiUrl.Contains("/api/core/system/v1/capabilities"))
        {
            DirectCallUI<bool>(UICommand.RobotReady, false);
        }

        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.NetError, $"API=> {apiUrl} 發生 {error}"));
    }
}
