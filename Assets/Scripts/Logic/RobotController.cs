using UnityEngine;

using XPlan;
using XPlan.Observe;

public class RobotReadyMsg : MessageBase
{
    public RobotReadyMsg()
    {

    }
}

public class RobotMoveMsg : MessageBase
{
    public string poiName;
    public RobotMoveMsg(string poiName)
    {
        this.poiName = poiName;
    }
}

public class RobotController : LogicComponent
{    
    public RobotController()
    {
        // ED TODO
        // 需要判斷機器人準備完成，才能送出Msg
        SendMsg<RobotReadyMsg>();

        // 要求機器人開始移動
        RegisterNotify<RobotMoveMsg>((msg) => 
        {
            new RobotMoveAPI(msg.poiName, (moveResponse) => 
            {
                // ED TODO
                // 檢查訊息有異常要發出錯誤

                if (moveResponse?.State == null)
                {
                    // ED TODO
                    // 沒有狀態，當錯誤處理
                }

                if (moveResponse.State.Status == 2)
                {
                    // ED TODO
                    // 狀態碼為 2 = 失敗
                }

                if (moveResponse.State.Status == 1 && moveResponse.State.Result != 0)
                {
                    // ED TODO
                    // 狀態碼成功，但 result != 0 也可以當作警告/錯誤
                }

                // ED TODO
                // 其他情況 (執行中 or 成功) 視為沒有錯誤                
            });
        });

        // 要求機器人回充電樁
        AddUIListener(UIRequest.BackToHomedock, () =>
        {
            new RobotBackToChargAPI((moveResponse) =>
            {
                // ED TODO
                // 確認回覆
            });
        });

        // 要求機器人停止動作
        AddUIListener(UIRequest.StopMoving, () =>
        {
            new RobotStopAPI();
        });
    }
}
