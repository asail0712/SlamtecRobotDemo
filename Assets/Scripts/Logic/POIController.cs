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

    public POIController()
    {
        RegisterNotify<RobotReadyMsg>((msg) =>
        {
            new GetPOIsAPI((poiList) => 
            {
                infoList = ToInfo(poiList);

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
            // ED TODO
            // 無效的idx要提出顯示

            return;
        }

        SendMsg<RobotMoveMsg>(infoList[idx].displayName);
    }

    private List<POIInfo> ToInfo(Poi[] poiList)
    {
        List<POIInfo> pOIInfos = new List<POIInfo>();

        foreach(Poi poi in poiList)
        {
            POIInfo pOIInfo = new POIInfo() 
            {
                uniqueID    = poi.Id,
                displayName = poi.DisplayName,
            };

            pOIInfos.Add(pOIInfo);
        }

        return pOIInfos;
    }
}
