using System;
using Newtonsoft.Json;

using XPlan.Net;

public class RobotStopAPI : DelWebRequest
{
    public RobotStopAPI()
    {
        SetUrl(APIDefine.BaseUrl + "/api/core/motion/v1/actions/:current");

        SendWebRequest();
    }
}
