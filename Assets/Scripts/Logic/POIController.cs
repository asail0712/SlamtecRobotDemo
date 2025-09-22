using System.Collections.Generic;
using System.Xml;
using UnityEngine;

using XPlan;
using XPlan.Observe;
using XPlan.Utility;

public class POIToMoveMsg : MessageBase
{
    public string poiName;

    public POIToMoveMsg(string poiName)
    {
        this.poiName = poiName;
    }
}

public class POIController : LogicComponent
{
    private List<POIInfo> infoList = null;

    private static int MAX_ITEM = 8;

    public POIController(bool bIgnoreRobot)
    {
        if (bIgnoreRobot)
        {
            DirectCallUI<bool>(UICommand.RobotReady, true); // 要完POI資料再開放UI操作
            return;
        }

        /****************************
         * 初始化左側POI列表
         * *************************/
        infoList = new List<POIInfo>();

        for(int i = 0; i < MAX_ITEM; ++i)
        {
            POIInfo info = new POIInfo() 
            {
                displayName = "無地點",
                bEnable     = false,
            };

            infoList.Add(info);
        }

        DirectCallUI<List<POIInfo>>(UICommand.SetPOIInfo, infoList);

        /***************************************
         * 等機器人正常運作後，要求POI內容
         * ************************************/
        RegisterNotify<RobotReadyMsg>((msg) =>
        {
            new GetPOIsAPI((poiList) => 
            {
                ToPOIInfo(poiList);

                foreach(POIInfo info in infoList)
                {
                    SendMsg<AddCommandMsg>(CommandType.MoveTo, info.displayName);
                }

                DirectCallUI<List<POIInfo>>(UICommand.SetPOIInfo, infoList);
                DirectCallUI<bool>(UICommand.RobotReady, true); // 要完POI資料再開放UI操作
            });
        });

        /***************************************
         * 透過POI名稱做移動
         * ************************************/
        RegisterNotify<POIToMoveMsg>((msg) => 
        {
            // 透過POI名稱 找出移動的地點
            int idx = infoList.FindIndex((E04) =>
            {
                return E04.displayName == msg.poiName;
            });

            MoveTo(idx);
        });

        /***************************************
         * 透過POI ID 做移動(使用者點擊UI)
         * ************************************/
        AddUIListener<string>(UIRequest.POIToMove, (poiID) => 
        {
            // 透過POI uid 找出移動的地點
            int idx = infoList.FindIndex((E04) => 
            {
                return E04.uniqueID == poiID;
            });

            MoveTo(idx);
        });
    }

    private void MoveTo(int idx)
    {
        if (infoList == null || !infoList.IsValidIndex(idx))
        {
            // 無效的idx要提出顯示
            DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.ClientError, $"無效的POI地點"));

            return;
        }

        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"將前往 {infoList[idx].displayName}"));

        SendMsg<RobotMoveMsg>(infoList[idx].displayName);
    }

    private void ToPOIInfo(Poi[] poiList)
    {
        if(infoList == null)
        { 
            return; 
        }

        for(int i = 0; i < poiList.Length; ++i)
        {
            if(i >= MAX_ITEM)
            {
                break;
            }

            POIInfo pOIInfo         = infoList[i];
            pOIInfo.uniqueID        = poiList[i].Id;
            pOIInfo.displayName     = poiList[i].DisplayName;
            pOIInfo.bEnable         = true;
        }
    }
}
