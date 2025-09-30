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
    public Pose pose;
    public RobotMoveMsg(Pose pose)
    {
        this.pose = pose;
    }
}

public class RobotChargingMsg : MessageBase
{
    public RobotChargingMsg()
    {
    }
}


public class RobotStopMsg : MessageBase
{
    public RobotStopMsg()
    {
    }
}


public class RobotController : LogicComponent
{    
    public RobotController(bool bIgnoreRobotInitial)
    {       
        /****************************
         * 強制初始化
         * *************************/
        //InitialRobot();

        /****************************
         * 要求機器人開始移動
         * *************************/
        RegisterNotify<RobotMoveMsg>((msg) => 
        {
            new RobotMoveAPI(msg.pose, MoveCallback);
        });

        /****************************
         * 要求機器人回充電樁
         * *************************/
        RegisterNotify<RobotChargingMsg>((msg) =>
        {
            new RobotBackToChargAPI(MoveCallback);
        });

        AddUIListener(UIRequest.BackToHomedock, () =>
        {
            new RobotBackToChargAPI(MoveCallback);
        });

        /****************************
         * 要求機器人停止動作
         * *************************/
        RegisterNotify<RobotStopMsg>((msg) =>
        {
            new RobotStopAPI();
        });

        AddUIListener(UIRequest.StopMoving, () =>
        {
            new RobotStopAPI();
        });

        /****************************
         * Robot initial
         * *************************/
        AddUIListener(UIRequest.RobotInitial, () =>
        {            
            if(bIgnoreRobotInitial)
            {
                return;
            }

            InitialRobot();
        });

        RegisterNotify<RobotInitialMsg>((dummy) =>
        {
            if (bIgnoreRobotInitial)
            {
                return;
            }

            InitialRobot();
        });
    }

    private void InitialRobot()
    {
        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotRequest, $"機器人狀態檢查"));

        new RobotCapabilitiesAPI((respList) => 
        {
            foreach(CapabilitiesRepsonse resp in respList)
            {
                if (!resp.Enabled)
                {
                    // 告知錯誤
                    DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"{resp.Name} 尚未完成初始化"));
                    DirectCallUI<bool>(UICommand.RobotReady, false);
                    return;
                }
            }

            new RobotPowerStatusAPI((resp) =>
            {
                if(resp.PowerStage != "running")
                {
                    // 告知錯誤
                    DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"機器人開機尚未完成 {resp.PowerStage}"));
                    DirectCallUI<bool>(UICommand.RobotReady, false);
                    return;
                }

                if(resp.SleepMode != "awake")
                {
                    // 告知錯誤 並主動 wake up
                    new RobotWakeUpAPI();
                    DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"機器人尚未喚醒，正在喚醒中 {resp.SleepMode}"));
                    DirectCallUI<bool>(UICommand.RobotReady, false);
                    return;
                }

                if(resp.BatteryPercentage <= 15)
                {
                    // 告知錯誤 但是不中斷流程
                    DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotWarning, $"機器人電量不足 {resp.BatteryPercentage}/100"));
                }

                new RobotNavigationStatusAPI((b) => 
                {
                    if(!b)
                    {
                        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"機器人定位尚未完成"));
                        DirectCallUI<bool>(UICommand.RobotReady, false);
                        return;
                    }

                    SendMsg<RobotReadyMsg>();
                });
            });
        });
    }

    private void MoveCallback(MoveResponse moveResponse)
    {
        if (moveResponse?.State == null)
        {
            // 沒有狀態，當錯誤處理
            DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"移動狀態(moveResponse.State.Status)為空"));
            return;
        }

        if (moveResponse.State.Status == 2)
        {
            // 狀態碼為 2 = 失敗
            DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"移動狀態(moveResponse.State.Status)為2"));
            return;
        }

        if (moveResponse.State.Status == 1 && moveResponse.State.Result != 0)
        {
            // 狀態碼成功，但 result != 0 也可以當作警告/錯誤
            DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.RobotError, $"移動結果(moveResponse.State.Result)異常 {moveResponse.State.Result}"));
            return;
        }

        // 其他情況 (執行中 or 成功) 視為沒有錯誤                
        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"機器人開始動作"));
    }
}
