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
        InitialRobot();

        // 要求機器人開始移動
        RegisterNotify<RobotMoveMsg>((msg) => 
        {
            DirectCallUI<string>(UICommand.AddMessage, $"[Robot Request] 機器人移動");

            new RobotMoveAPI(msg.poiName, MoveCallback);
        });

        // 要求機器人回充電樁
        AddUIListener(UIRequest.BackToHomedock, () =>
        {
            DirectCallUI<string>(UICommand.AddMessage, $"[Robot Request] 回到充電站");

            new RobotBackToChargAPI(MoveCallback);
        });

        // 要求機器人停止動作
        AddUIListener(UIRequest.StopMoving, () =>
        {
            DirectCallUI<string>(UICommand.AddMessage, $"[Robot Request] 機器人停止動作");

            new RobotStopAPI();
        });

        // 重新initial
        AddUIListener(UIRequest.Reinitial, () =>
        {            
            InitialRobot();
        });
    }

    private void MoveCallback(MoveResponse moveResponse)
    {
        if (moveResponse?.State == null)
        {
            // 沒有狀態，當錯誤處理
            DirectCallUI<string>(UICommand.AddMessage, $"[Robot Error] 移動狀態(moveResponse.State.Status)為空");
            return;
        }

        if (moveResponse.State.Status == 2)
        {
            // 狀態碼為 2 = 失敗
            DirectCallUI<string>(UICommand.AddMessage, $"[Robot Error] 移動狀態(moveResponse.State.Status)為2");
            return;
        }

        if (moveResponse.State.Status == 1 && moveResponse.State.Result != 0)
        {
            // 狀態碼成功，但 result != 0 也可以當作警告/錯誤
            DirectCallUI<string>(UICommand.AddMessage, $"[Robot Error] 移動結果(moveResponse.State.Result)異常 {moveResponse.State.Result}");
            return;
        }

        // 其他情況 (執行中 or 成功) 視為沒有錯誤                
        DirectCallUI<string>(UICommand.AddMessage, $"[Log] 機器人開始動作");
    }

    private void InitialRobot()
    {
        DirectCallUI<string>(UICommand.AddMessage, $"[Robot Request] 機器人狀態檢查");

        new RobotPowerStatusAPI((resp) =>
        {
            if(resp.PowerStage != "running")
            {
                // 告知錯誤
                DirectCallUI<string>(UICommand.AddMessage, $"[Robot Error] 機器人開機尚未完成 {resp.PowerStage}");
                DirectCallUI<bool>(UICommand.RobotReady, false);
                return;
            }

            if(resp.SleepMode != "awake")
            {
                // 告知錯誤 並主動 wake up
                new RobotWakeUpAPI();
                DirectCallUI<string>(UICommand.AddMessage, $"[Robot Error] 機器人尚未喚醒，正在喚醒中 {resp.SleepMode}");
                DirectCallUI<bool>(UICommand.RobotReady, false);
                return;
            }

            if(resp.BatteryPercentage <= 15)
            {
                // 告知錯誤 但是不中斷流程
                DirectCallUI<string>(UICommand.AddMessage, $"[Robot Warning] 機器人電量不足 {resp.BatteryPercentage}/100");
            }

            new RobotNavigationStatusAPI((b) => 
            {
                if(!b)
                {
                    DirectCallUI<string>(UICommand.AddMessage, $"[Robot Error] 機器人定位尚未完成");
                    return;
                }

                SendMsg<RobotReadyMsg>();

                DirectCallUI<bool>(UICommand.RobotReady, true);
            });
        });
    }
}
