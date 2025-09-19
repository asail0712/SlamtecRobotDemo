using System.Collections.Generic;
using UnityEngine;
using XPlan.UI;
using XPlan.UI.Component;

public class POIInfo : TableItemInfo
{
    public string displayName   = "";
    public bool bEnable         = true;
}

public class POI_UI : UIBase
{
    [SerializeField] GameObject itemAnchor;
    [SerializeField] GameObject itemPrefab;

    private VerticalTableManager<POIInfo> poiTableMgr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        poiTableMgr = new VerticalTableManager<POIInfo>();

        poiTableMgr.InitTable(itemAnchor, 10, 2, itemPrefab);
        poiTableMgr.SetChildAlignment(TextAnchor.UpperCenter);
        poiTableMgr.SetGridSpacing(0, 50);

        ListenCall<List<POIInfo>>(UICommand.SetPOIInfo, (infoList) =>
        {
            poiTableMgr.SetInfoList(infoList);
            poiTableMgr.Refresh();
        });
    }
}
