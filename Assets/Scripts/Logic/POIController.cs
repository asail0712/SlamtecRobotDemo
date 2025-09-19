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

    public POIController()
    {
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

        RegisterNotify<RobotReadyMsg>((msg) =>
        {
            new GetPOIsAPI((poiList) => 
            {
                AppendInfo(poiList);

                DirectCallUI<List<POIInfo>>(UICommand.SetPOIInfo, infoList);
            });
        });

        RegisterNotify<POIToMoveMsg>((msg) => 
        {
            // 透過POI名稱 找出移動的地點
            int idx = infoList.FindIndex((E04) =>
            {
                return E04.displayName == msg.poiName;
            });

            MoveTo(idx);
        });

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
            DirectCallUI<string>(UICommand.AddMessage, $"[System Error] 無效的POI地點");

            return;
        }

        DirectCallUI<string>(UICommand.AddMessage, $"[Log] 將前往 {infoList[idx].displayName}");

        SendMsg<RobotMoveMsg>(infoList[idx].displayName);
    }

    private void AppendInfo(Poi[] poiList)
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
